using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Xunit;
using SomatologyClinic.Models;

namespace SomatologyClinic.Tests
{
    public class BookingTests
    {
        [Fact]
        public void UserId_ShouldBeRequired()
        {
            var property = typeof(Booking).GetProperty("UserId");
            var attribute = property.GetCustomAttribute<RequiredAttribute>();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TreatmentId_ShouldBeRequired()
        {
            var property = typeof(Booking).GetProperty("TreatmentId");
            var attribute = property.GetCustomAttribute<RequiredAttribute>();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void BookingDateTime_ShouldBeRequired()
        {
            var property = typeof(Booking).GetProperty("BookingDateTime");
            var attribute = property.GetCustomAttribute<RequiredAttribute>();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void BookingDateTime_ShouldHaveDataTypeDateTime()
        {
            var property = typeof(Booking).GetProperty("BookingDateTime");
            var attribute = property.GetCustomAttribute<DataTypeAttribute>();
            Assert.NotNull(attribute);
            Assert.Equal(DataType.DateTime, attribute.DataType);
        }

        [Fact]
        public void Status_ShouldDefaultToPending()
        {
            var booking = new Booking();
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact]
        public void StaffNotes_ShouldBeNullable()
        {
            var property = typeof(Booking).GetProperty("StaffNotes");
            Assert.True(Nullable.GetUnderlyingType(property.PropertyType) != null || property.PropertyType == typeof(string));
        }

        [Fact]
        public void Price_ShouldBeNullable()
        {
            var property = typeof(Booking).GetProperty("Price");
            Assert.True(Nullable.GetUnderlyingType(property.PropertyType) != null);
        }
    }
}
