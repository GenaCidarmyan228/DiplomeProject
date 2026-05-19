using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiplomeProject; 

namespace CheckLoginTests
{
    [TestClass]
    public class LoginCheckerTests
    {
        [TestMethod]
        public void Check_LoginWithoutSpecSymbols_ReturnsTrue()
        {
            Assert.IsTrue(AuthChecker.ValidateLogin("Admin123"));
        }

        [TestMethod]
        public void Check15Symbols_ReturnsFalse()
        {
            Assert.IsFalse(AuthChecker.ValidateLogin("Admin1234567890")); // 15 символов
        }

        [TestMethod]
        public void Check2Symbols_ReturnsFalse()
        {
            Assert.IsFalse(AuthChecker.ValidateLogin("Ad")); // 2 символа
        }

        [TestMethod]
        public void Check4Symbols_ReturnTrue()
        {
            Assert.IsTrue(AuthChecker.ValidateLogin("Ivan")); // 4 символа
        }

        [TestMethod]
        public void CheckLoginWithSpecialSymbols_ReturnsFalse()
        {
            Assert.IsFalse(AuthChecker.ValidateLogin("Admin@123")); // Содержит @
        }
    }

    [TestClass]
    public class PasswordCheckerTests
    {
        [TestMethod]
        public void Check_CorrectPassword()
        {
            Assert.IsTrue(AuthChecker.ValidatePassword("ValidPass123"));
        }

        [TestMethod]
        public void Check_PasswordWithSpectSymbols_ReturnsFalse()
        {
            // Пароль состоит только из спецсимволов (нет букв и цифр)
            Assert.IsFalse(AuthChecker.ValidatePassword("@@@@@@@@"));
        }

        [TestMethod]
        public void Check_PasswordWithSpectSymbols_ReturnsTrue()
        {
            // Корректный пароль со спецсимволом
            Assert.IsTrue(AuthChecker.ValidatePassword("Passw@rd1"));
        }

        [TestMethod]
        public void Check10symbols_ReturnFalse()
        {
            // 10 символов, но нет цифр
            Assert.IsFalse(AuthChecker.ValidatePassword("HelloWorld"));
        }

        [TestMethod]
        public void Check10Symbols_ReturnTrue()
        {
            // 10 символов, корректный
            Assert.IsTrue(AuthChecker.ValidatePassword("HelloWo123"));
        }

        [TestMethod]
        public void Check29Symbols_ReturnsFalse()
        {
            // 29 символов
            Assert.IsFalse(AuthChecker.ValidatePassword("ThisPasswordIsWayTooLong12345"));
        }

        [TestMethod]
        public void Check8Symbols_ReturnsTrue()
        {
            // Ровно 8 символов
            Assert.IsTrue(AuthChecker.ValidatePassword("Pass1234"));
        }

        [TestMethod]
        public void Check9SYMBOLS_ReturnFalse()
        {
            // 9 символов, но нет букв
            Assert.IsFalse(AuthChecker.ValidatePassword("123456789"));
        }

        [TestMethod]
        public void Check9SYMBOLS_ReturnTrue()
        {
            // 9 символов, корректный
            Assert.IsTrue(AuthChecker.ValidatePassword("Pass12345"));
        }
    }
}