﻿using EvernoteClone.Model;
using EvernoteClone.ViewModels.Commands;
using EvernoteClone.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EvernoteClone.ViewModels
{
    public class NotesViewModel : INotifyPropertyChanged
    {
        public NotesViewModel()
        {
            NewNotebookCommand = new NewNotebookCommand(this);
            NewNoteCommand = new NewNoteCommand(this);

            Notebooks = new ObservableCollection<Notebook>();
            GetNotebooks();

            Notes = new ObservableCollection<Note>();
            FontFamilies = new ObservableCollection<FontFamily>(Fonts.SystemFontFamilies.OrderBy(f => f.Source));
            SelectedFontFamily = Fonts.SystemFontFamilies.First();
            FontSizes = new ObservableCollection<double>(new List<double> { 8, 9, 11, 13, 14, 16, 20, 24, 32, 48, 72 });
            GetNotes();
        }

        private void GetNotebooks()
        {
            Notebooks.Clear();
            DatabaseHelper.Read<Notebook>().ForEach(Notebooks.Add);
        }

        private void GetNotes()
        {
            if (SelectedNotebook is null)
                return;

            Notes.Clear();
            DatabaseHelper.Read<Note>()
                          .Where(note => note.NotebookId == SelectedNotebook.Id)
                          .ToList()
                          .ForEach(Notes.Add);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private Notebook _selectedNotebook;

        public Notebook SelectedNotebook
        {
            get { return _selectedNotebook; }
            set
            {
                _selectedNotebook = value;
                GetNotes();
                OnPropertyChanged();
            }
        }


        private string _statusBarMessage;
        private FontFamily selectedFontFamily;
        private double _selectedFontSize;

        public string StatusBarMessage
        {
            get { return _statusBarMessage; }
            set
            {
                _statusBarMessage = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<Notebook> Notebooks { get; init; }

        public ObservableCollection<Note> Notes { get; init; }

        public ObservableCollection<FontFamily> FontFamilies { get; init; }

        public ObservableCollection<double> FontSizes { get; init; }

        public FontFamily SelectedFontFamily
        {
            get => selectedFontFamily; set
            {
                selectedFontFamily = value;
                OnPropertyChanged();
            }
        }
        public double SelectedFontSize
        {
            get => _selectedFontSize; set
            {
                _selectedFontSize = value;
                OnPropertyChanged();
            }
        }

        public NewNotebookCommand NewNotebookCommand { get; }

        public NewNoteCommand NewNoteCommand { get; }

        public void NewNotebook()
        {
            DatabaseHelper.Insert(new Notebook() { Name = "Undefined" });
            GetNotebooks();
        }

        public void NewNote(Notebook notebook)
        {
            DatabaseHelper.Insert(
                new Note()
                {
                    Title = $"Note for {DateTime.Now}",
                    NotebookId = notebook.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });

            GetNotes();
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propName = "") 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

    }
}
