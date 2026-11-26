# Карта проекта для AI-агентов

Поддерживайте этот файл в актуальном состоянии при существенных изменениях структуры проекта.

## Обзор

CharacterCreationIM — Unity-приложение для создания, редактирования, развития и печати персонажей Imperium Maledictum. Подробности о возможностях и стеке находятся в `.ai-factory/DESCRIPTION.md`.

## Технологии

- Язык: C#.
- Движок: Unity `6000.0.60f1`.
- Интерфейс и зависимости: Unity UI, TextMesh Pro, Zenject, R3, UniTask.
- Хранение: JSON-файлы в `Assets/StreamingAssets`; базы данных и ORM нет.

## Структура

```text
Assets/
  Scenes/                    сцена GameScene.unity
  _project/
    Scripts/                 код приложения
      Creators/              загрузка игровых справочников
      Factories/             создание экранов и презентеров
      Installers/            регистрация зависимостей Zenject
      Mediators/             сценарии переходов между экранами
      Presenters/            логика экранов
      View/                  компоненты интерфейса и печать листов
    Prefabs/                 префабы интерфейса
    SO/                      ScriptableObject-ресурсы приложения
  StreamingAssets/           игровые JSON и сохранённые персонажи
  Plugins/                  сторонние плагины
Packages/                    зависимости Unity
ProjectSettings/             настройки Unity
docs/                        документация проекта
.agents/skills/              навыки AI Factory для Codex app
.ai-factory/                 контекст и настройки AI Factory
```

## Ключевые точки входа

| Файл | Назначение |
|------|------------|
| `Assets/Scenes/GameScene.unity` | Сцена приложения. |
| `Assets/_project/Scripts/View/Bootstrap.cs` | Загружает справочники и открывает главное меню. |
| `Assets/_project/Scripts/Mediators/StartMediator.cs` | Обрабатывает действия главного меню. |
| `Assets/_project/Scripts/Installers/CreatorInstaller.cs` | Регистрирует загрузчики справочников. |
| `Assets/_project/Scripts/CharacterStorage.cs` | Сохраняет и загружает персонажа в JSON. |
| `Packages/manifest.json` | Зависимости Unity. |

## Документация

| Документ | Путь | Содержание |
|----------|------|------------|
| README | `README.md` | Краткое описание проекта. |
| Таланты | `docs/talents.md` | Данные талантов и действующие игровые эффекты. |
| Покровители | `docs/patrons-json.md` | Поля пяти JSON-справочников и порядок добавления записей и эффектов. |

## Контекст AI Factory

| Файл | Назначение |
|------|------------|
| `AGENTS.md` | Карта проекта. |
| `.ai-factory/config.yaml` | Язык, пути и настройки рабочих процессов. |
| `.ai-factory/DESCRIPTION.md` | Возможности и технологии проекта. |
| `.ai-factory/ARCHITECTURE.md` | Архитектурные правила. |
| `.ai-factory/rules/base.md` | Обнаруженные соглашения кода. |

## Правила работы

- Учитывайте существующие изменения в рабочем дереве Git и не перезаписывайте их без необходимости.
- Выполняйте команды оболочки по отдельности; не соединяйте их через `&&`, `||` или `;`.
- При изменении игрового JSON сверяйте формат с кодом в `Creators`; при изменении экранов проверяйте префабы и регистрации Zenject.
