﻿using System.Collections.Generic;
using System.Linq;
using Framework;

namespace Domain
{
    public sealed class ConsecutiveNumberIntervals : ValueObject
    {
        private readonly List<ConsecutiveNumberInterval> _sortedConsecutiveNumberIntervals = new List<ConsecutiveNumberInterval>
        {
            ConsecutiveNumberInterval.NewFor(0)
        };
        
        public static ConsecutiveNumberIntervals New() => new ConsecutiveNumberIntervals();
        
        public void MarkAsApplied(long eventNumber)
        {
            var newSet = ConsecutiveNumberInterval.NewFor(eventNumber);
            var insertPosition = FindInsertPositionFor(newSet);
            
            _sortedConsecutiveNumberIntervals.Insert(insertPosition, newSet);

            TryMergeRightWithMiddle(insertPosition);
            TryMergeLeftWithMiddle(insertPosition);
        }
        
        private int FindInsertPositionFor(ConsecutiveNumberInterval set)
        {
            var binarySearchPosition = _sortedConsecutiveNumberIntervals.BinarySearch(set);
            return binarySearchPosition >= 0 ? binarySearchPosition : ~binarySearchPosition;
        }

        private void TryMergeRightWithMiddle(int position)
        {
            if (RightIntervalExistsFor(position))
            {
                if (_sortedConsecutiveNumberIntervals[position].TryMerge(
                    _sortedConsecutiveNumberIntervals[position + 1],
                    out var mergedSet))
                {
                    _sortedConsecutiveNumberIntervals[position] = mergedSet;
                    _sortedConsecutiveNumberIntervals.RemoveAt(position + 1);
                }
            }
        }

        private bool RightIntervalExistsFor(int position) => position + 1 < _sortedConsecutiveNumberIntervals.Count;

        private void TryMergeLeftWithMiddle(int position)
        {
            if (LeftIntervalExistsFor(position))
            {
                if (_sortedConsecutiveNumberIntervals[position - 1].TryMerge(
                    _sortedConsecutiveNumberIntervals[position],
                    out var mergedSet))
                {
                    _sortedConsecutiveNumberIntervals[position - 1] = mergedSet;
                    _sortedConsecutiveNumberIntervals.RemoveAt(position);
                }
            }
        }

        private static bool LeftIntervalExistsFor(int position) => position > 0;

        public long LastConsecutiveAppliedEventNumber => _sortedConsecutiveNumberIntervals.First().Tail;
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var item in _sortedConsecutiveNumberIntervals) yield return item;
        }
    }
}