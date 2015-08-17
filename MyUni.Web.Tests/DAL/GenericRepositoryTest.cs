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
                new Student {Id = 1, FirstName = "A"},
                new Student {Id = 2, FirstName = "B"},
                new Student {Id = 3, FirstName = "C"}
            };
        }

        private Mock<DbSet<Student>> GetStudentDbSet(List<Student> students)
        {
            students = students ?? new List<Student>();

            var mockedStudentDbSet = new Mock<DbSet<Student>>();
            mockedStudentDbSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.AsQueryable().Provider);
            mockedStudentDbSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.AsQueryable().Expression);
            mockedStudentDbSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.AsQueryable().ElementType);
            mockedStudentDbSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.GetEnumerator());

            return mockedStudentDbSet;
        }

        private DbContext GetMyUniDbContext(List<Student> students )
        {
            students = students ?? new List<Student>();

            var studentDbSet = GetStudentDbSet(students);

            var dbContext = new Mock<MyUniDbContext>();
            dbContext.Setup(x => x.Set<Student>()).Returns(studentDbSet.Object);

            return dbContext.Object;
        }

        [TestMethod]
        public void GetAll_ReturnsAll()
        {
            var students = GetStudentList();
            var context = GetMyUniDbContext(students);
            var repository = new Mock<GenericRepository<Student>>(context);
            repository.Setup(x => x.GetAll()).Returns(students.AsQueryable());

            var list = repository.Object.GetAll();

            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count());
            Assert.AreEqual(students[0], list.First());
        }

        [TestMethod]
        public void GetById_Existing_Id_Returns_Not_Null()
        {
            //
            // Arrange
            //
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
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
        public void GetById_Id_Not_Present_Returns_Null()
        {
            //
            // Arrange
            //
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
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
            Assert.IsNull(student, "The student cannot exist");
        }

        [TestMethod]
        public void Add_ValidEntity_Must_Not_Return_Null()
        {
            //
            // Arrange
            //
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
            mockedRepository.Setup(x => x.Add(It.IsAny<Student>())).Returns((Student s) =>
            {
                if (s == null)
                {
                    return null;
                }

                studentList.Add(s);
                return s;
            });
            var newStudent = new Student {FirstName = "D"};
            //
            // Act
            //
            var student = mockedRepository.Object.Add(newStudent);
            //
            // Assert
            //
            Assert.AreEqual(4, studentList.Count);
            Assert.IsNotNull(student, "The student does not exist");
        }

        [TestMethod]
        public void Add_Null_Entity_Must_Return_Null()
        {
            //
            // Arrange
            //
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
            mockedRepository.Setup(x => x.Add(It.IsAny<Student>())).Returns((Student s) =>
            {
                if (s == null)
                {
                    return null;
                }

                studentList.Add(s);
                return s;
            });
            
            Student newStudent = null;
            //
            // Act
            //
            var student = mockedRepository.Object.Add(newStudent);
            //
            // Assert
            //
            Assert.IsNull(student);
        }

        [TestMethod]
        public void Delete_Existing_Entity_Must_Reduce_Collection_Size()
        {
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
            mockedRepository.Setup(x => x.Delete(It.IsAny<Student>())).Callback((Student s) =>
            {
                studentList.Remove(s);
            });

            var studentToRemove = studentList[0];

            mockedRepository.Object.Delete(studentToRemove);

            Assert.AreEqual(2,studentList.Count);
            Assert.AreNotEqual("A", studentList[0].FirstName);
        }

        [TestMethod]
        public void Delete_Non_Existing_Entity_Must_Not_Affect_Collection()
        {
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
            mockedRepository.Setup(x => x.Delete(It.IsAny<Student>())).Callback((Student s) =>
            {
                studentList.Remove(s);
            });

            var studentToRemove = new Student{FirstName = "AAA"};

            mockedRepository.Object.Delete(studentToRemove);

            Assert.AreEqual(3, studentList.Count);
        }

        [TestMethod]
        public void Delete_Existing_Id_Must_Reduce_Collection_Size()
        {
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
            mockedRepository.Setup(x => x.Delete(It.IsAny<int>())).Callback((int id) =>
            {
                studentList.Remove(studentList.FirstOrDefault(x => x.Id == id));
            });

            var studentToRemove = studentList[0];

            mockedRepository.Object.Delete(studentToRemove.Id);

            Assert.AreEqual(2, studentList.Count);
            Assert.AreNotEqual("A", studentList[0].FirstName);
        }

        [TestMethod]
        public void Delete_Non_Existing_Id_Must_Not_Affect_Collection()
        {
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
            mockedRepository.Setup(x => x.Delete(It.IsAny<int>())).Callback((int id) =>
            {
                studentList.Remove(studentList.FirstOrDefault(x => x.Id == id));
            });

            var studentToRemove = new Student { Id = 999 };

            mockedRepository.Object.Delete(studentToRemove.Id);

            Assert.AreEqual(3, studentList.Count);
        }

        [TestMethod]
        public void Update_Existing_Entity_Must_Be_Updated()
        {
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
            mockedRepository.Setup(x => x.Update(It.IsAny<Student>())).Callback((Student student) =>
            {
                var findIndex = studentList.FindIndex(x => x.Id == student.Id);
                if (findIndex < 0)
                {
                    return;
                }
                studentList[findIndex] = student;
            });
            var studentToUpdate = studentList[0];
            studentToUpdate.FirstName = "AAA";

            mockedRepository.Object.Update(studentToUpdate);

            Assert.AreEqual(studentList[0], studentToUpdate);
            Assert.AreEqual("AAA", studentList[0].FirstName);

        }

        [TestMethod]
        public void Update_Non_Existing_Entity_Must_Not_Be_Updated()
        {
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
            mockedRepository.Setup(x => x.Update(It.IsAny<Student>())).Callback((Student student) =>
            {
                var findIndex = studentList.FindIndex(x => x.Id == student.Id);
                if (findIndex < 0)
                {
                    return;
                }

                studentList[findIndex] = student;
            });
            var studentToUpdate = new Student{FirstName = "AAA"};
            

            mockedRepository.Object.Update(studentToUpdate);

            Assert.AreEqual(studentList[0].FirstName, "A");
        }


        [TestMethod]
        public void Get_Valid_Filter_Must_Return_Data()
        {
            var studentList = GetStudentList();
            var context = GetMyUniDbContext(studentList);
            var mockedRepository = new Mock<GenericRepository<Student>>(context);
            mockedRepository.Setup(x => x.Get(It.IsAny<Func<Student, bool>>())).Returns((Func<Student, bool> filter) =>
            {
                if (filter == null)
                {
                    return null;
                }

                return studentList.Where(filter).AsQueryable();
            });
            Func<Student,bool> testFilter = (student) => student.FirstName == "A";

            var filteredStudents = mockedRepository.Object.Get(testFilter);

            Assert.IsNotNull(filteredStudents);
            Assert.AreEqual(1, filteredStudents.Count());
            Assert.AreEqual(studentList[0].FirstName, filteredStudents.First().FirstName);

        }



    }
}
