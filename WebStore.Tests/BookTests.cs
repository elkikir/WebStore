using NPOI.SS.Formula.Functions;

namespace WebStore.Tests
{
    public class BookTests
    {
        [Fact]
        public void IsIsbn_WithNull_ReturnFalse()
        {
            bool actual = Book.IsIsbn(null);

            Assert.False(actual);
        }

        [Fact]
        public void IsIsbn_WithBlankString_ReturnFalse()
        {
            bool actual = Book.IsIsbn("     ");

            Assert.False(actual);
        }

        [Fact]
        public void IsIsbn_WithInvalidIsbn_ReturnFalse()
        {
            bool actual = Book.IsIsbn("ISBN 123");

            Assert.False(actual);
        }

        [Fact]
        public void IsIsbn_WithIsbn10_ReturnTrue()
        {
            bool actual = Book.IsIsbn("IsBn 100-100-100 1");

            Assert.True(actual);
        }

        [Fact]
        public void IsIsbn_WithIsbn13_ReturnTrue()
        {
            bool actual = Book.IsIsbn("IsBn 100-100 100 2112");

            Assert.True(actual);
        }

        [Fact]
        public void IsIsbn_WithTrashInside_ReturnFalse()
        {
            bool actual = Book.IsIsbn("xxx IsBn 100-100 100 211 xxx");

            Assert.False(actual);
        }
    }
}