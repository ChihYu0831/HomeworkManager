using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using HomeworkManager.Models;

namespace HomeworkManager.Services
{
    public class HomeworkService
    {
        private readonly string _filePath;
        private List<Homework> _homeworks;
        private readonly JavaScriptSerializer _serializer;

        public HomeworkService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "HomeworkManager");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "homeworks.json");
            _serializer = new JavaScriptSerializer();
            _homeworks = Load();
        }

        private List<Homework> Load()
        {
            if (!File.Exists(_filePath)) return new List<Homework>();
            try
            {
                string json = File.ReadAllText(_filePath);
                return _serializer.Deserialize<List<Homework>>(json) ?? new List<Homework>();
            }
            catch { return new List<Homework>(); }
        }

        private void Save()
        {
            string json = _serializer.Serialize(_homeworks);
            File.WriteAllText(_filePath, json);
        }

        public List<Homework> GetAll() => _homeworks.ToList();

        public List<Homework> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return GetAll();
            keyword = keyword.ToLower();
            return _homeworks.Where(h =>
                h.CourseName.ToLower().Contains(keyword) ||
                h.Title.ToLower().Contains(keyword)).ToList();
        }

        public void Add(Homework homework)
        {
            _homeworks.Add(homework);
            Save();
        }

        public void Update(Homework homework)
        {
            var index = _homeworks.FindIndex(h => h.Id == homework.Id);
            if (index >= 0)
            {
                _homeworks[index] = homework;
                Save();
            }
        }

        public void Delete(string id)
        {
            _homeworks.RemoveAll(h => h.Id == id);
            Save();
        }

        public void MarkCompleted(string id)
        {
            var hw = _homeworks.FirstOrDefault(h => h.Id == id);
            if (hw != null)
            {
                hw.IsCompleted = true;
                Save();
            }
        }
    }
}
