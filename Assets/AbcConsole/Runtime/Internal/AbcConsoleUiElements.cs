using AnKuchen.KuchenLayout;
using AnKuchen.KuchenLayout.Layouter;
using AnKuchen.KuchenList;
using AnKuchen.Map;
using UnityEngine.UI;

namespace AbcConsole.Internal
{
    public class AbcConsoleUiElements
    {
        public Console Console { get; private set; }
        public Button TriggerButton { get; private set; }
        public InputField InputField { get; private set; }
        public VerticalList<LogLineUiElements, LogDetailUiElements> Log { get; private set; }
        public Layout<AutocompleteItemUiElements> Autocomplete { get; private set; }

        public AbcConsoleUiElements(IMapper mapper)
        {
            Initialize(mapper);
        }

        private void Initialize(IMapper mapper)
        {
            Console = mapper.Get<Console>("Console");
            TriggerButton = mapper.Get<Button>("TriggerButton");
            Log = new VerticalList<LogLineUiElements, LogDetailUiElements>(
                mapper.Get<ScrollRect>("Log"),
                mapper.GetChild<LogLineUiElements>("Log/LogLine"),
                mapper.GetChild<LogDetailUiElements>("Log/LogDetail")
            );
            InputField = mapper.Get<InputField>("Input/InputField");
            Autocomplete = new Layout<AutocompleteItemUiElements>(
                mapper.GetChild<AutocompleteItemUiElements>("Autocomplete/Item"),
                new BottomToTopLayouter(2.5f)
            );
        }

        public class AutocompleteItemUiElements : IReusableMappedObject
        {
            public IMapper Mapper { get; private set; }
            public Button Button { get; private set; }
            public Text Text { get; private set; }

            public void Initialize(IMapper mapper)
            {
                Mapper = mapper;
                Button = mapper.Get<Button>();
                Text = mapper.Get<Text>("Text");
            }

            public void Activate()
            {
            }

            public void Deactivate()
            {
                Button.onClick.RemoveAllListeners();
            }
        }

        public class LogLineUiElements : IReusableMappedObject
        {
            public IMapper Mapper { get; private set; }
            public Button Button { get; private set; }
            public Text Text { get; private set; }

            public void Initialize(IMapper mapper)
            {
                Mapper = mapper;
                Button = mapper.Get<Button>();
                Text = mapper.Get<Text>("Text");
            }

            public void Activate()
            {
            }

            public void Deactivate()
            {
                Button.onClick.RemoveAllListeners();
            }
        }

        public class LogDetailUiElements : IReusableMappedObject
        {
            public IMapper Mapper { get; private set; }

            public void Initialize(IMapper mapper)
            {
                Mapper = mapper;
            }

            public void Activate()
            {
            }

            public void Deactivate()
            {
            }
        }
    }
}