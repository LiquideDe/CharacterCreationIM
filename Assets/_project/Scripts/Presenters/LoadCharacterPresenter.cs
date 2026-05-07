using Newtonsoft.Json;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class LoadCharacterPresenter : IPresenter, IInitializable
    {
        //[Inject] private IFactory<LoadCharacterView> _factory;
        private CompositeDisposable _cd = new CompositeDisposable();
        private AudioManager _audioManager;
        private LoadCharacterView _view;
        private Subject<Character> _returnCharacter = new Subject<Character>();
        private Subject<Unit> _returnToMenu = new Subject<Unit>();


        public LoadCharacterPresenter(AudioManager audioManager, LoadCharacterView view)
        {
            _audioManager = audioManager;
            _view = view;
        }

        public Observable<Character> LoadedCharacter => _returnCharacter;
        public Observable<Unit> ReturnToMenu => _returnToMenu;

        public void ShowSaves()
        {
            _view.SetEntries(ScanFolderAndShowCharacters());
        }

        private void LoadCharacter(Entry entry)
        {
            var character = new Character();
            Debug.LogAssertion($"entry == null {entry == null}, entry.Header == null {entry.Header == null} ");
            CharacterMapper.ApplyDto(character, entry.Header);
            Close(() => _returnCharacter.OnNext(character));
        }

        private List<Entry> ScanFolderAndShowCharacters()
        {
            var root = Path.Combine(Application.streamingAssetsPath, "Персонажи");

            if (!Directory.Exists(root))
            {
                Debug.LogWarning($"[CharacterFileScanner] Папка не найдена: {root}");
            }

            var paths = Directory.EnumerateFiles(root, "*.json", SearchOption.AllDirectories);
            var list = new List<Entry>();
            foreach (var p in paths)
            {
                try
                {
                    var fi = new FileInfo(p);
                    var e = new Entry
                    {
                        FullPath = p,
                        FileName = Path.GetFileNameWithoutExtension(p),
                        SizeBytes = fi.Length,
                        LastWriteUtc = fi.LastWriteTimeUtc
                    };

                        try
                        {
                        // Загружаем как DTO (используем те же настройки, что и в CharacterStorage)
                            var json = File.ReadAllText(p);
                            var dto = JsonConvert.DeserializeObject<CharacterDto>(json, new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto,
                                NullValueHandling = NullValueHandling.Ignore
                            });
                        // нормализуем коллекции, чтобы не было null
                            CharacterStorage.EnsureCollections(dto);
                            e.Header = dto;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogAssertion($"[CharacterFileScanner] Не удалось распарсить '{p}': {ex.Message}");
                        }
                    

                    list.Add(e);
                }
                catch (Exception ex)
                {
                    Debug.LogAssertion($"[CharacterFileScanner] Пропускаю '{p}': {ex}");
                }                
            }      
            return list;
        }

        private void Close(Action onComplete = null)
        {
            _view.HideAndDestroyToRight(() => onComplete?.Invoke());
            _cd.Clear();
        }

        public void Initialize()
        {
            _view.OnSaveClicked.Subscribe(entry => { LoadCharacter(entry); }).AddTo(_cd);
            _view.OnButtonCloseClicked.Subscribe(_ => { Close(); _returnToMenu.OnNext(Unit.Default); }).AddTo(_cd);
        }

        public void Dispose()
        {
            _cd.Dispose();
        }
    }

    public sealed class Entry
    {
        public string FullPath;        // полный путь к json
        public string FileName;        // имя файла без расширения
        public long SizeBytes;
        public DateTime LastWriteUtc;
        public CharacterDto Header;    // если validate=true и парс прошёл — тут загруженный dto
    }
}

