﻿using Domain;
using FluentAssertions;
using Xunit;

namespace UnitTests
{
    public sealed class ConsecutiveNumberIntervalsPerformanceTests
    {
        [Fact]
        public void simulate_best_case_scenario()
        {
            var numberCount = 1000000;
            var intervals = ConsecutiveNumberIntervals.New();
            
            for (int i = 1; i <= numberCount; ++i)
            {
                intervals.Insert(new DomainEventPosition(i, i, i));
                var unused = intervals.LargestConsecutiveNumber;
            }
            
            intervals.LargestConsecutiveNumber.Should().Be(new DomainEventPosition(numberCount, numberCount, numberCount));
        }

        [Fact ]
        public void simulate_worst_case_scenario()
        {
            var numberCount = 1000000;
            var intervals = ConsecutiveNumberIntervals.New();
            
            for (int i = 2; i <= numberCount; i += 2)
            {
                intervals.Insert(new DomainEventPosition(i, i, i));
                var unused = intervals.LargestConsecutiveNumber;
            }
            
            for (int i = 1; i <= numberCount; i += 2)
            {
                intervals.Insert(new DomainEventPosition(i, i, i));
                var unused = intervals.LargestConsecutiveNumber;
            }

            intervals.LargestConsecutiveNumber.Should().Be(new DomainEventPosition(numberCount, numberCount, numberCount));
        }
        
        [Theory]
        [InlineData(1000000, 1000, 10)]
        [InlineData(1000000, 100, 10)]
        public void simulate_some_realistic_scenario(
            long numberCount,
            int windowSize,
            int skipInterval)
        {
            var intervals = ConsecutiveNumberIntervals.New();
            for (long i = 0; i < numberCount; i += windowSize)
            {
                for (var j = i + 1; j <= i + windowSize; ++j)
                    if (j % skipInterval != 0)
                    {
                        intervals.Insert(new DomainEventPosition(j, j, j));
                         var unused = intervals.LargestConsecutiveNumber;
                    }
                for (var j = i + 1; j <= i + windowSize; ++j)
                    if (j % skipInterval == 0)
                    {
                        intervals.Insert(new DomainEventPosition(j, j, j));
                        var unused = intervals.LargestConsecutiveNumber;
                    }
            }
            intervals.LargestConsecutiveNumber.Should().Be(new DomainEventPosition(numberCount, numberCount, numberCount));
        }
        
    }
}