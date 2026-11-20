using System;
using MedClinic.Models;
using MedClinic.Services;
using Xunit;

namespace MedClinic.Tests
{
    /// <summary>Проверка правил ввода: ФИО, телефон, логин, цена, время.</summary>
    public class ValidationTests
    {
        [Theory]
        [InlineData("Иванов Иван", true)]
        [InlineData("Петрова-Водкина Анна-Мария", true)]
        [InlineData("Иванов", false)]
        [InlineData("Иванов Иван 3", false)]
        [InlineData("", false)]
        public void IsValidFullName_Works(string value, bool expected)
        {
            Assert.Equal(expected, Validation.IsValidFullName(value));
        }

        [Theory]
        [InlineData("+7 (900) 111-22-33", true)]
        [InlineData("8-900-111-22-33", true)]
        [InlineData("12345", false)]
        [InlineData("телефон", false)]
        public void IsValidPhone_Works(string value, bool expected)
        {
            Assert.Equal(expected, Validation.IsValidPhone(value));
        }

        [Theory]
        [InlineData("admin", true)]
        [InlineData("manager_1", true)]
        [InlineData("ab", false)]
        [InlineData("админ", false)]
        [InlineData("user name", false)]
        public void IsValidLogin_Works(string value, bool expected)
        {
            Assert.Equal(expected, Validation.IsValidLogin(value));
        }

        [Theory]
        [InlineData("12345", true)]
        [InlineData("1234", false)]
        public void IsValidPassword_Works(string value, bool expected)
        {
            Assert.Equal(expected, Validation.IsValidPassword(value));
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1500.50, true)]
        [InlineData(-1, false)]
        [InlineData(1000001, false)]
        public void IsValidPrice_Works(double price, bool expected)
        {
            Assert.Equal(expected, Validation.IsValidPrice((decimal)price));
        }

        [Theory]
        [InlineData("08:00", true)]
        [InlineData("19:30", true)]
        [InlineData("20:00", true)]
        [InlineData("07:30", false)]
        [InlineData("25:00", false)]
        [InlineData("8:00", false)]
        public void IsValidWorkingTime_Works(string time, bool expected)
        {
            Assert.Equal(expected, Validation.IsValidWorkingTime(time));
        }

        [Fact]
        public void CheckDoctor_ReturnsErrorForBadName()
        {
            var doctor = new Doctor { FullName = "Иванов", Specialty = "Терапевт" };
            Assert.NotNull(Validation.CheckDoctor(doctor));
        }

        [Fact]
        public void CheckDoctor_AllowsDoctorWithoutOptionalFields()
        {
            var doctor = new Doctor { FullName = "Иванов Иван", Specialty = "Терапевт" };
            Assert.Null(Validation.CheckDoctor(doctor));
        }

        [Fact]
        public void CheckService_ReturnsErrorForBadDuration()
        {
            var service = new Service { Name = "Приём", Price = 1000, Duration = 1 };
            Assert.NotNull(Validation.CheckService(service));
        }

        [Fact]
        public void CheckService_AllowsValidService()
        {
            var service = new Service { Name = "Приём терапевта", Price = 1500, Duration = 30 };
            Assert.Null(Validation.CheckService(service));
        }
    }
}
