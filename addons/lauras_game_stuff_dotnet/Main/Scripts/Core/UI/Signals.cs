public static class Signals {
    public static class Object {
        public const string
            PROPERTY_LIST_CHANGED = "property_list_changed",
            SCRIPT_CHANGED = "script_changed";
    }
    
    public static class Node {
        public const string
            CHILD_ENTERED_TREE = "child_entered_tree",
            CHILD_EXITING_TREE = "child_exiting_tree",
            CHILD_ORDER_CHANGED = "child_order_changed",
            EDITOR_DESCRIPTION_CHANGED = "editor_description_changed",
            READY = "ready",
            RENAMED = "renamed",
            REPLACING_BY = "replacing_by",
            TREE_ENTERED = "tree_entered",
            TREE_EXITED = "tree_exited",
            TREE_EXITING = "tree_exiting";
    }
    
    public static class CanvasItem {
        public const string
            DRAW = "draw",
            HIDDEN = "hidden",
            ITEM_RECT_CHANGED = "item_rect_changed",
            VISIBILITY_CHANGED = "visibility_changed";
    }
    
    public static class Control {
        public const string
            FOCUS_ENTERED = "focus_entered",
            FOCUS_EXITED = "focus_exited",
            GUI_INPUT = "gui_input",
            MINIMUM_SIZE_CHANGED = "minimum_size_changed",
            MOUSE_ENTERED = "mouse_entered",
            MOUSE_EXITED = "mouse_exited",
            RESIZED = "resized",
            SIZE_FLAGS_CHANGED = "size_flags_changed",
            THEME_CHANGED = "theme_changed";
    }
    
    public static class Container {
        public const string
            PRE_SORT_CHILDREN = "pre_sort_children",
            SORT_CHILDREN = "sort_children";
    }
    
    public static class Button {
        public const string
            PRESSED = "pressed",
            BUTTON_DOWN = "button_down",
            BUTTON_UP = "button_up",
            TOGGLED = "toggled";
    }
    
    public static class TextEdit {
        public const string
            CARET_CHANGED = "caret_changed",
            GUTTER_ADDED = "gutter_added",
            GUTTER_CLICKED = "gutter_clicked",
            GUTTER_REMOVED = "gutter_removed",
            LINES_EDITED_FROM = "lines_edited_from",
            TEXT_CHANGED = "text_changed",
            TEXT_SET = "text_set";
    }
    
    public static class GraphEdit {
        // TODO: Add GraphEdit signals
    }
    
    public static class ItemList {
        public const string
            EMPTY_CLICKED = "empty_clicked",
            ITEM_ACTIVATED = "item_activated",
            ITEM_CLICKED = "item_clicked",
            ITEM_SELECTED = "item_selected",
            MULTI_SELECTED = "multi_selected";
    }
    
    public static class LineEdit {
        public const string
            TEXT_CHANGE_REJECTED = "text_change_rejected",
            TEXT_CHANGED = "text_changed",
            TEXT_SUBMITTED = "text_submitted";
    }
    
    // TODO: Add more signals when I can be fucked :CryingEliza:
}