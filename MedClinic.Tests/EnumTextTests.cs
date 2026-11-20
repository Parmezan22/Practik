using System;
using MedClinic.Models;
using MedClinic.Services;
using Xunit;

namespace MedClinic.Tests
{
    /// <summary>Перевод ролей и статусов между базой данных и экраном.</summary>
    public class EnumTextTests
    {
        [Theory]
        [InlineData(UserRole.Guest, "гость")]
        [InlineData(UserRole.Client, "клиент")]
        [InlineData(UserRole.Manager, "менеджер")]
        [InlineData(UserRole.Admin, "админ")]
        public void Role_RoundTrip(UserRole role, string dbText)
        {
            Assert.Equal(dbText, EnumText.RoleToDb(role));
            Assert.Equal(role, EnumText.RoleFromDb(dbText));
        }

        [Theory]
        [InlineData(AppointmentStatus.Booked, "записан", "Записан")]
        [InlineData(AppointmentStatus.Cancelled, "отменён", "Отменён")]
        [InlineData(AppointmentStatus.Completed, "завершён", "Завершён")]
        public void Status_RoundTrip(AppointmentStatus status, string dbText, string display)
        {
            Assert.Equal(dbText, EnumText.StatusToDb(status));
            Assert.Equal(status, EnumText.StatusFromDb(dbText));
            Assert.Equal(display, EnumText.StatusToDisplay(status));
        }

        [Fact]
        public void UnknownRole_BecomesGuest()
        {
            Assert.Equal(UserRole.Guest, EnumText.RoleFromDb("неизвестная роль"));
            Assert.Equal(UserRole.Guest, EnumText.RoleFromDb(null));
        }

        [Fact]
        public void UnknownStatus_BecomesBooked()
        {
            Assert.Equal(AppointmentStatus.Booked, EnumText.StatusFromDb("???"));
        }

        [Fact]
        public void RoleFromDb_IgnoresCaseAndSpaces()
        {
            Assert.Equal(UserRole.Manager, EnumText.RoleFromDb("  МЕНЕДЖЕР "));
        }
    }

    /// <summary>Хэширование паролей алгоритмом SHA256.</summary>
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_ReturnsKnownSha256()
        {
            // SHA256("admin123") — эталонное значение
            Assert.Equal("240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9",
                PasswordHasher.Hash("admin123"));
        }

        [Fact]
        public void Hash_IsStableAndLowercase()
        {
            var first = PasswordHasher.Hash("Пароль1");
            var second = PasswordHasher.Hash("Пароль1");

            Assert.Equal(first, second);
            Assert.Equal(64, first.Length);
            Assert.Equal(first.ToLowerInvariant(), first);
        }

        [Fact]
        public void Verify_AcceptsCorrectPassword()
        {
            var hash = PasswordHasher.Hash("manager123");
            Assert.True(PasswordHasher.Verify("manager123", hash));
            Assert.True(PasswordHasher.Verify("manager123", hash.ToUpperInvariant()));
        }

        [Fact]
        public void Verify_RejectsWrongPassword()
        {
            var hash = PasswordHasher.Hash("manager123");
            Assert.False(PasswordHasher.Verify("manager124", hash));
            Assert.False(PasswordHasher.Verify("", hash));
            Assert.False(PasswordHasher.Verify("manager123", ""));
        }

        [Fact]
        public void Hash_RejectsEmptyPassword()
        {
            Assert.Throws<ArgumentException>(delegate { PasswordHasher.Hash(""); });
        }
    }

    /// <summary>Матрица прав доступа по ролям.</summary>
    public class PermissionsTests
    {
        [Fact]
        public void Guest_CannotCreateAppointment()
        {
            Assert.False(Permissions.CanCreateAppointment(UserRole.Guest));
            Assert.True(Permissions.CanCreateAppointment(UserRole.Client));
        }

        [Fact]
        public void Reports_OnlyForManagerAndAdmin()
        {
            Assert.False(Permissions.CanViewReports(UserRole.Guest));
            Assert.False(Permissions.CanViewReports(UserRole.Client));
            Assert.True(Permissions.CanViewReports(UserRole.Manager));
            Assert.True(Permissions.CanViewReports(UserRole.Admin));
        }

        [Fact]
        public void Users_OnlyForAdmin()
        {
            Assert.False(Permissions.CanManageUsers(UserRole.Manager));
            Assert.True(Permissions.CanManageUsers(UserRole.Admin));
        }
    }
}
