using System;
namespace UE_Assistant
{
    public class Grade
    {
        public string Subject { get; set; }
        public string TypeOfSubject { get; set; }
        public string GradeValue { get; set; }
        public int AmountOfECTS { get; set; }
        public DateTime DateOfGrade { get; set; }
        public int NumberOfTerm { get; set; }
        public string NameOfTeacher { get; set; }
        public string TypeOfSubjectPass { get; set; }

        public Grade(string subject, string typeOfSubject, string gradeValue, int amountOfECTS, 
                     DateTime dateOfGrade, int numberOfTerm, string nameOfTeacher, string typeOfSubjectPass)
        {
            this.Subject = subject;
            this.TypeOfSubject = typeOfSubject;
            this.GradeValue = gradeValue;
            this.AmountOfECTS = amountOfECTS;
            this.DateOfGrade = dateOfGrade;
            this.NumberOfTerm = numberOfTerm;
            this.NameOfTeacher = nameOfTeacher;
            this.TypeOfSubjectPass = typeOfSubjectPass;
        }

        new public string ToString()
        {
            return string.Format(
            "Przedmiot: {0}" +
            "\nTyp: {1}" +
            "\nOcena: {2}" +
            "\nPunkty ECTS: {3}" +
            "\nData wystawienia: {4}" +
            "\nNumer terminu: {5}" +
            "\nNauczyciel: {6}" +
            "\nTyp zaliczenia: {7}",
            Subject,TypeOfSubject,GradeValue,AmountOfECTS,DateOfGrade.ToShortDateString(),
            NumberOfTerm,NameOfTeacher,TypeOfSubjectPass);
        }
    }
}
