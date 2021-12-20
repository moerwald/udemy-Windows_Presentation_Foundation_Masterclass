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

        private Notebook _selectedNotebook;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Notebook SelectedNotebook
        {
            get { return _selectedNotebook; }
            set
            {
                _selectedNotebook = value;
                GetNotes();
                OnPropertyChanged(nameof(SelectedNotebook));
            }
        }

        public ObservableCollection<Notebook> Notebooks { get; init; }
        public ObservableCollection<Note> Notes { get; init; }

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
                    NotebookId = notebook.Id ,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });

            GetNotes();
        }

        private void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

    }
}