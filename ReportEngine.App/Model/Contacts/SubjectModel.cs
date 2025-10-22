using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model.Contacts
{
    public class SubjectModel : BaseViewModel
    {
        private ObservableCollection<Subject> _allSubjects;
        private string subjectName;
        private string companyName;
        private Subject _selectedSubject;

        public ObservableCollection<Subject> AllSubjects
        {
            get => _allSubjects;
            set => Set(ref _allSubjects, value);
        }

        public string SubjectName
        {
            get => subjectName;
            set => Set(ref subjectName, value);
        }

        public string CompanyName
        {
            get => companyName;
            set => Set(ref companyName, value);
        }

        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set => Set(ref _selectedSubject, value);
        }

        public Subject CreateNewSubject()
        {
            return new Subject
            {
                ObjectName = SubjectName,
                CompanyName = CompanyName,
            };
        }
    }
}
