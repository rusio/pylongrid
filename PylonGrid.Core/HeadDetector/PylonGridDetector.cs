using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ExtensionMethods;

namespace PylonGrid.Core
{
	/// <summary>
	/// A head detector that marks and finds human heads, that
	/// are represented as local minima in the range image.
	/// 
	/// Uses a static grid of pylons, that are placed uniformly over 
	/// the range image. Every pylon, that is on a further plane than
	/// one of his neighbours is considered tail (non-head). All the 
	/// neighbours of that tail pylon, that are on the same range, or
	/// even further are also marked as tail. At the end, only the
	/// head pylons remain untouched. 
	/// </summary>
	public sealed class PylonGridDetector : IHeadDetector
	{
		private readonly int _ImgWidth;
		private readonly int _ImgHeight;
		private readonly int _PyStep; // pixels between pylons

		private readonly IMedianFilter _MedianFilter;

		// the grid holding all pylons
		private readonly Pylon[,] _Grid;
		private readonly int _GridRows;
		private readonly int _GridCols;
		
		public PylonGridDetector(int imgWidth, int imgHeight, int pyStep) 
			: this(imgWidth, imgHeight, pyStep, new NullMedianFilter())
		{
		}
		
		public PylonGridDetector(int imgWidth, int imgHeight, int pyStep, IMedianFilter medianFilter)
		{
			_ImgWidth = imgWidth;
			_ImgHeight = imgHeight;
			_PyStep = Math.Max(pyStep, 3);  // avoid StackOverflowException
			_MedianFilter = medianFilter;

			_GridRows = _ImgHeight / _PyStep;
			_GridCols = _ImgWidth / _PyStep;
			_Grid = new Pylon[_GridRows, _GridCols];
			PlacePylons();
		}
		
		private void PlacePylons()
		{
			for (int gridRow = 0; gridRow < _GridRows; gridRow++) {
				for (int gridCol = 0; gridCol < _GridCols; gridCol++) {
					PlacePylon(gridRow, gridCol);
				}
			}
		}

		private void PlacePylon(int gridRow, int gridCol)
		{
			int halfStep = _PyStep / 2;
			int imgX = (gridCol * _PyStep) + halfStep;
			int imgY = (gridRow * _PyStep) + halfStep;
			bool isBorder = (gridRow == 0 || gridRow == _GridRows - 1 ||
			                   gridCol == 0 || gridCol == _GridCols - 1);
			Pylon pylon = new Pylon(imgX, imgY, isBorder);
			_Grid[gridRow, gridCol] = pylon;
		}

		public List<Head> DetectHeads(Bitmap rangeImage)
		{
			ResetPylons(rangeImage);
			DiscardTailPylons();
			List<Head> heads = GroupHeadPylons();
			return heads;
		}

		private unsafe void ResetPylons(Bitmap rangeImage)
		{
			BitmapData rangeData = rangeImage.LockReadOnly();
			byte* scan0 = (byte*)rangeData.Scan0;
			int stride = rangeData.Stride;

			for (int gridRow = 1; gridRow < _GridRows - 1; gridRow++) {
				for (int gridCol = 1; gridCol < _GridCols - 1; gridCol++) {
					Pylon pylon = _Grid[gridRow, gridCol];
					ResetPylon(pylon, scan0, stride);
				}
			}
			rangeImage.UnlockBits(rangeData);
		}

		private unsafe void ResetPylon(Pylon pylon, byte* scan0, int stride)
		{
			byte range = *(scan0 + pylon.ImgY * stride + pylon.ImgX);
			if (pylon.IsInsideImage(_MedianFilter.Radius, _ImgWidth, _ImgHeight)) {
				range = _MedianFilter.Calculate(pylon.ImgX, pylon.ImgY, scan0, stride);
			}
			pylon.ResetAt(range);
		}
		        
		private void DiscardTailPylons()
		{
			for (int gridRow = 1; gridRow < _GridRows - 1; gridRow++) {
				for (int gridCol = 1; gridCol < _GridCols - 1; gridCol++) {
					DiscardTailPylon(gridRow, gridCol, null);
				}
			}
		}

		private void DiscardTailPylon(int gridRow, int gridCol, Pylon previous)
		{
			Pylon current = _Grid[gridRow, gridCol];
			if (!current.IsHead)
				return;
			
			Pylon adjacentE = _Grid[gridRow, gridCol + 1];
			Pylon adjacentW = _Grid[gridRow, gridCol - 1];
			Pylon adjacentN = _Grid[gridRow - 1, gridCol];
			Pylon adjacentS = _Grid[gridRow + 1, gridCol];
			
			bool visitedE = (previous == adjacentE) || current.IsDiscardedBy(adjacentE);
			bool visitedW = (previous == adjacentW) || current.IsDiscardedBy(adjacentW);
			bool visitedN = (previous == adjacentN) || current.IsDiscardedBy(adjacentN);
			bool visitedS = (previous == adjacentS) || current.IsDiscardedBy(adjacentS);

			if (visitedE || visitedN || visitedW || visitedS) {
				current.IsHead = false;
				if (!visitedE)  // go E
					DiscardTailPylon(gridRow, gridCol + 1, current);
				if (!visitedN)  // go N
					DiscardTailPylon(gridRow - 1, gridCol, current);
				if (!visitedW)  // go W
					DiscardTailPylon(gridRow, gridCol - 1, current);
				if (!visitedS)  // go S
					DiscardTailPylon(gridRow + 1, gridCol, current);
			}
		}

		private List<Head> GroupHeadPylons()
		{
			List<Head> heads = new List<Head>();
			for (int gridRow = 1; gridRow < _GridRows - 1; gridRow++) {
				for (int gridCol = 1; gridCol < _GridCols - 1; gridCol++) {
					PylonGroup pylonGroup = new PylonGroup();
					GroupHeadPylon(gridRow, gridCol, ref pylonGroup);
					if (pylonGroup.PylonCount > 0) {
						Head head = pylonGroup.ToHead();
						heads.Add(head);
					}
				}
			}
			return heads;
		}

		private void GroupHeadPylon(int gridRow, int gridCol, ref PylonGroup pylonGroup)
		{
			Pylon pylon = _Grid[gridRow, gridCol];
			if (!pylon.IsHead || pylon.IsGrouped)
				return;

			pylonGroup.Register(pylon);

			GroupHeadPylon(gridRow - 1, gridCol, ref pylonGroup);
			GroupHeadPylon(gridRow + 1, gridCol, ref pylonGroup);
			GroupHeadPylon(gridRow, gridCol - 1, ref pylonGroup);
			GroupHeadPylon(gridRow, gridCol + 1, ref pylonGroup);
		}

		public void DebugVisualize(Bitmap debugImage, List<Head> heads)
		{
			// visualize the pylons
			for (int gridRow = 0; gridRow < _GridRows; gridRow++) {
				for (int gridCol = 0; gridCol < _GridCols; gridCol++) {
					Pylon pylon = _Grid[gridRow, gridCol];
					Color color = (pylon.IsBorder ? Color.Brown : (pylon.IsHead ? Color.YellowGreen : Color.DarkSlateBlue));
					debugImage.SetPixel(pylon.ImgX, pylon.ImgY, color);
				}
			}

			// visualize the heads
			foreach (Head head in heads) {
				Color color = Color.YellowGreen;
				debugImage.SetPixel(head.CenterX, head.CenterY, color);
				debugImage.SetPixel(head.CenterX - 1, head.CenterY, color);
				debugImage.SetPixel(head.CenterX + 1, head.CenterY, color);
				debugImage.SetPixel(head.CenterX, head.CenterY - 1, color);
				debugImage.SetPixel(head.CenterX, head.CenterY + 1, color);
			}
		}
	}
}