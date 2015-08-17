using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyUni.Business;
using MyUni.DAL;
using MyUni.DAL.Concrete;

namespace MyUni.Web.Tests.DAL
{
    [TestClass]
    public class GenericRepositoryTest
    {

        private List<Student> GetStudentList()
        {
            return new List<Student>
            {
                new Student {Id = 1, FirstName = "Cheranga"},
                new Student {Id = 2, FirstName = "Murali"},
                new Student {Id = 3, FirstName = "Van"}
            };
        }

        private Mock<DbSet<Student>> GetMockedStudentDbSet(List<Student> students)
        {
            students = students ?? new List<Student>();

            var mockedStudentDbSet = new Mock<DbSet<Student>>();
            mockedStudentDbSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.AsQueryable().Provider);
            mockedStudentDbSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.AsQueryable().Expression);
            mockedStudentDbSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.AsQueryable().ElementType);
            mockedStudentDbSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.GetEnumerator());

            return mockedStudentDbSet;
        }

        [TestMethod]
        public void GetAll()
        {
            //
            // Arrange
            //
            var studentList = GetStudentList();

            var mockStudentDbSet = GetMockedStudentDbSet(studentList);

            var myUniDbContext = new Mock<MyUniDbContext>();
            myUniDbContext.Setup(x => x.Set<Student>()).Returns(mockStudentDbSet.Object);
            myUniDbContext.Setup(x => x.Students).Returns(mockStudentDbSet.Object);

            var genericRepository = new GenericRepository<Student>(myUniDbContext.Object);
            //
            // Act
            //
            var students = genericRepository.GetAll();
            //
            // Assert
            //
            Assert.AreEqual(3, students.Count(), "All students are not returned");
            Assert.AreEqual("Cheranga", students.First().FirstName, "The data is not the same");
        }

        [TestMethod]
        public void GetById_Existing_Id_Must_Not_Be_Null()
        {
            //
            // Arrange
            //
            var studentList = GetStudentList();
            var mockStudentDbSet = GetMockedStudentDbSet(studentList);


            var myUniDbContext = new Mock<MyUniDbContext>();
            myUniDbContext.Setup(x => x.Set<Student>()).Returns(mockStudentDbSet.Object);
            myUniDbContext.Setup(x => x.Students).Returns(mockStudentDbSet.Object);


            var mockedRepository = new Mock<GenericRepository<Student>>(myUniDbContext.Object);
            mockedRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns<int>((x) =>
            {
                return studentList.FirstOrDefault(y => y.Id == x);
            });
            //
            // Act
            //
            var student = mockedRepository.Object.GetById(1);
            //
            // Assert
            //
            Assert.IsNotNull(student, "The student does not exist");
        }

        [TestMethod]
        public void GetById_Id_Not_Present_Must_Return_Null()
        {
            //
            // Arrange
            //
            var studentList = GetStudentList();
            var mockStudentDbSet = GetMockedStudentDbSet(studentList);
           
            var myUniDbContext = new Mock<MyUniDbContext>();
            myUniDbContext.Setup(x => x.Set<Student>()).Returns(mockStudentDbSet.Object);
            myUniDbContext.Setup(x => x.Students).Returns(mockStudentDbSet.Object);


            var mockedRepository = new Mock<GenericRepository<Student>>(myUniDbContext.Object);
            mockedRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns<int>((x) =>
            {
                return studentList.FirstOrDefault(y => y.Id == x);
            });
            //
            // Act
            //
            var student = mockedRepository.Object.GetById(5);
            //
            // Assert
            //
            Assert.IsNull(student, "The student exists");
        }

        [TestMethod]
        public void Add_ValidEntity()
        {
            //
            // Arrange
            //
            var studentList = GetStudentList();
            var mockStudentDbSet = GetMockedStudentDbSet(studentList);


            var myUniDbContext = new Mock<MyUniDbContext>();
            myUniDbContext.Setup(x => x.Set<Student>()).Returns(mockStudentDbSet.Object);
            myUniDbContext.Setup(x => x.Students).Returns(mockStudentDbSet.Object);

            var mockedRepository = new Mock<GenericRepository<Student>>(myUniDbContext.Object);
            mockedRepository.Setup(x => x.Add(It.IsAny<Student>())).Callback((Student student) =>
            {
                studentList.Add(student);
            });
            //
            // Act
            //

            //
            // Assert
            //
        }

    }
}
