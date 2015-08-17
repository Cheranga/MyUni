using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyUni.Business;
using MyUni.DAL.Abstract;

namespace MyUni.Web.Tests.DAL
{
    [TestClass]
    public class RepositoryInterfaceTests
    {
        private List<Student> GetStudentList()
        {
            return new List<Student>
            {
                new Student {Id = 1, FirstName = "A"},
                new Student {Id = 2, FirstName = "B"},
                new Student {Id = 3, FirstName = "C"},
            };
        }
        public void GetAll()
        {
            var studentList = GetStudentList();
            var repository = new Mock<IRepository<Student>>();
            repository.Setup(x => x.GetAll()).Returns(studentList.AsQueryable());

            var students = repository.Object.GetAll();

            Assert.AreEqual(3, students.Count());
            Assert.AreEqual("A", students.First().FirstName);
        }

        [TestMethod]
        public void GetById_With_Existing_Id()
        {
            var studentList = GetStudentList();
            var repository = new Mock<IRepository<Student>>();
            repository.Setup(x => x.GetById(It.IsAny<int>())).Returns((int id) =>
            {
                return studentList.FirstOrDefault(student => student.Id == id);
            });

            var cheranga = repository.Object.GetById(1);

            Assert.IsNotNull(cheranga);
            Assert.AreEqual(cheranga, studentList[0]);
        }

        [TestMethod]
        public void GetById_With_NON_Existing_Id()
        {
            var studentList = GetStudentList();
            var repository = new Mock<IRepository<Student>>();
            repository.Setup(x => x.GetById(It.IsAny<int>())).Returns((int id) =>
            {
                return studentList.FirstOrDefault(student => student.Id == id);
            });

            var cheranga = repository.Object.GetById(5);

            Assert.IsNull(cheranga);
        }

        [TestMethod]
        public void Add()
        {
            var studentList = GetStudentList();
            var repository = new Mock<IRepository<Student>>();
            repository.Setup(x => x.Add(It.IsAny<Student>())).Returns((Student student) =>
            {
                studentList.Add(student);
                student.Id = studentList.Count;

                return student;
            });

            var newStudent = new Student
            {
                FirstName = "New Student"
            };

            repository.Object.Add(newStudent);

            Assert.AreEqual(4, studentList.Count);
            Assert.AreEqual(newStudent,studentList[3]);

        }

        [TestMethod]
        public void Update_Exisiting_Entity()
        {
            var studentList = GetStudentList();
            var repository = new Mock<IRepository<Student>>();
            repository.Setup(x => x.Update(It.IsAny<Student>())).Callback((Student student) =>
            {
                if (student == null)
                {
                    throw new NullReferenceException();
                }
                
                var currentStudent = studentList.FirstOrDefault(x => x.Id == student.Id);
                if (currentStudent == null)
                {
                    throw new NullReferenceException();
                }

                currentStudent.FirstName = student.FirstName;
            });
        }
    }
}
