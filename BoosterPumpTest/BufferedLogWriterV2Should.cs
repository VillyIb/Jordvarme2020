﻿using System;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using BoosterPumpLibrary.Logger;
using eu.iamia.Util;

namespace BoosterPumpTest
{
    public class BufferedLogWriterV2Should
    {
        readonly BufferedLogWriterV2 Sut;
        readonly IOutputFileHandler FakeFileHandler;

        public BufferedLogWriterV2Should()
        {
            // Arrange
            FakeFileHandler = Substitute.For<IOutputFileHandler>();
            Sut = new BufferedLogWriterV2(FakeFileHandler);
        }

        [Fact]
        public void IsNextMinute_ReturnsTrueThenFalse()
        {
            Assert.True(Sut.IsNextMinute());
            Assert.False(Sut.IsNextMinute());
        }

        [Fact]
        public void RoundToMinute_RemovesSecondsAndFractions()
        {
            var timestamp = DateTime.Today.AddHours(12).AddMinutes(12);
            Assert.Equal(timestamp, BufferedLogWriterV2.RoundToMinute(timestamp.AddSeconds(12).AddMilliseconds(12)));
        }

        private static void InitSystemDateTime(int minute)
        {
            SystemDateTime.SetTime(new DateTime(2000, 01, 01, 12, minute, 30, DateTimeKind.Utc), 0D);
        }

        private void SetupBufferLines()
        {
            InitSystemDateTime(10);
            Sut.Add(new BufferLine("A", SystemDateTime.UtcNow));
            InitSystemDateTime(11);
            Sut.Add(new BufferLine("B", SystemDateTime.UtcNow));
            Sut.Add(new BufferLine("C", SystemDateTime.UtcNow));
            InitSystemDateTime(12);
            Sut.Add(new BufferLine("D", SystemDateTime.UtcNow));
        }

        private void SetupBufferLineMeasurements()
        {
            InitSystemDateTime(10);
            Sut.Add(new BufferLineMeasurement(SystemDateTime.UtcNow, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f));
            InitSystemDateTime(11);
            Sut.Add(new BufferLineMeasurement(SystemDateTime.UtcNow, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f));
            Sut.Add(new BufferLineMeasurement(SystemDateTime.UtcNow, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f));
            InitSystemDateTime(12);
            Sut.Add(new BufferLineMeasurement(SystemDateTime.UtcNow, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f));
        }

        [Fact]
        public void AggregateFlush_WriteOlderEntriesAggregated()
        {
            SetupBufferLines();
            Sut.AggregateFlush(SystemDateTime.UtcNow);
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("T"), Arg.Is("A, B, C"));
        }


        [Fact]
        public void AggregateFlushUnconditionally_WriteAllEntriesAggregated()
        {
            SetupBufferLines();
            Sut.AggregateFlushUnconditionally();
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("T"), Arg.Is("A, B, C, D"));
        }

        [Fact]
        public void AggregateFlush_WriteOlderEntriesSpecificAndAggregated()
        {
            SetupBufferLineMeasurements();
            Sut.AggregateFlush((SystemDateTime.UtcNow));
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("M"), Arg.Any<string>());
            FakeFileHandler.Received(3).WriteLine(Arg.Any<DateTime>(), Arg.Is("S"), Arg.Any<string>());
        }

        [Fact]
        public void AggregateFlushUnconditionally_WriteAllEntriesSpecificAndAggregated()
        {
            SetupBufferLineMeasurements(); Sut.AggregateFlushUnconditionally();
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("M"), Arg.Any<string>());
            FakeFileHandler.Received(4).WriteLine(Arg.Any<DateTime>(), Arg.Is("S"), Arg.Any<string>());
        }

        [Fact]
        public async Task AggregateFlushAsync_WriteOlderEntriesAggregated()
        {
            SetupBufferLines();
            await Sut.AggregateFlushAsync(SystemDateTime.UtcNow);
            await FakeFileHandler.Received(1).WriteLineAsync(Arg.Any<DateTime>(), Arg.Is("T"), Arg.Is("A, B, C"));
        }

        [Fact]
        public async Task AggregateFlushUnconditionallyAsync_WriteAllEntriesAggregated()
        {
            SetupBufferLines();
            await Sut.AggregateFlushUnconditionalAsync();
            await FakeFileHandler.Received(1).WriteLineAsync(Arg.Any<DateTime>(), Arg.Is("T"), Arg.Is("A, B, C, D"));
        }

        [Fact]
        public async Task AggregateFlushAsync_WriteOlderSpecificAndAggregated()
        {
            SetupBufferLineMeasurements(); await Sut.AggregateFlushAsync(SystemDateTime.UtcNow);
            await FakeFileHandler.Received(1).WriteLineAsync(Arg.Any<DateTime>(), Arg.Is("M"), Arg.Any<string>());
            await FakeFileHandler.Received(3).WriteLineAsync(Arg.Any<DateTime>(), Arg.Is("S"), Arg.Any<string>());
        }

        [Fact]
        public async Task AggregateFlushUnconditionalAsync_WriteAllEntriesSpecificAndAggregated()
        {
            SetupBufferLineMeasurements(); await Sut.AggregateFlushUnconditionalAsync();
            await FakeFileHandler.Received(1).WriteLineAsync(Arg.Any<DateTime>(), Arg.Is("M"), Arg.Any<string>());
            await FakeFileHandler.Received(4).WriteLineAsync(Arg.Any<DateTime>(), Arg.Is("S"), Arg.Any<string>());
        }
    }
}

