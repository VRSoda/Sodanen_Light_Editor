using System.Collections.Generic;

namespace Brightness.Localization
{
    public static class English
    {
        public static readonly Dictionary<string, string> Strings = new()
        {
            // ===== Menu & Window =====
            { "menu.title", "Brightness Control Editor" },
            { "window.title", "Sodanen Editor" },

            // ===== Avatar Section =====
            { "avatar.title", "Avatar" },
            { "avatar.target", "Target" },
            { "avatar.select", "Please select an avatar" },
            { "avatar.select_dropdown", "-- Select Avatar --" },
            { "avatar.material_count", "lilToon Materials: {0}" },
            { "avatar.no_descriptor", "No VRCAvatarDescriptor" },

            // ===== Feature Section =====
            { "feature.title", "Feature Selection" },
            { "feature.light", "Light" },
            { "feature.shadow", "Shadow" },
            { "feature.minlight", "Min Light" },
            { "feature.maxlight", "Max Light" },
            { "feature.backlight", "Back Light" },
            { "feature.shadow_main", "Shadow" },
            { "feature.shadow_x", "Shadow X Angle" },
            { "feature.shadow_y", "Shadow Y Angle" },
            { "feature.minlight.tooltip", "Minimum brightness limit in dark environments" },
            { "feature.maxlight.tooltip", "Maximum brightness limit in bright environments" },
            { "feature.backlight.tooltip", "Intensity of light from behind" },
            { "feature.shadow.tooltip", "Shadow direction control" },
            { "feature.shadow_x.tooltip", "X-axis shadow direction" },
            { "feature.shadow_y.tooltip", "Y-axis shadow direction" },

            // ===== Material Settings =====
            { "material.title", "Material Settings" },
            { "material.window_title", "Material Editor" },
            { "material.unify_toggle", "Unify Properties / Individual Control" },
            { "material.select_all", "All" },
            { "material.deselect_all", "None" },

            // ===== Unify Section =====
            { "unify.title", "Unify Material Properties" },
            { "unify.check_before", "Check if material initial values are identical before applying" },
            { "unify.source_material", "Import from Material" },
            { "unify.shadow", "Shadow" },
            { "unify.mask_strength", "Mask and Strength" },
            { "unify.strength", "Strength" },
            { "unify.shadow_1", "Shadow Color 1" },
            { "unify.shadow_2", "Shadow Color 2" },
            { "unify.shadow_3", "Shadow Color 3" },
            { "unify.color", "Color" },
            { "unify.range", "Range" },
            { "unify.blur", "Blur" },
            { "unify.normal_strength", "Normal Map Strength" },
            { "unify.receive_shadow", "Receive Shadow" },
            { "unify.alpha", "Alpha" },
            { "unify.border_color", "Border Color" },
            { "unify.border_range", "Border Width" },
            { "unify.contrast", "Contrast" },
            { "unify.env_strength", "Environment Light Effect on Shadow" },
            { "unify.blur_mask", "Blur Effect Mask" },
            { "unify.compare", "Compare Before Modify" },
            { "unify.apply_material", "Apply Material" },

            // ===== Non-Destructive Mode =====
            { "nondestructive.title", "Non-Destructive Mode" },
            { "nondestructive.description", "Duplicates avatar and materials. Original remains unchanged." },
            { "nondestructive.duplicate_only", "Duplicate Only" },
            { "nondestructive.duplicate_unify", "Duplicate + Unify" },
            { "nondestructive.delete_all", "Delete Copy" },

            // ===== Custom Material =====
            { "custom.title", "Individual Material Shadow Control ({0})" },
            { "custom.add_help", "Add materials using the + button" },
            { "custom.exclude_notice", "These materials are excluded from 'Unify Material Properties'" },
            { "custom.select_material", "(Select Material)" },
            { "custom.material", "Material" },
            { "custom.current_value", "Current" },
            { "custom.shadow_1", "Shadow 1" },
            { "custom.shadow_2", "Shadow 2" },
            { "custom.shadow_3", "Shadow 3" },

            // ===== Preset Section =====
            { "preset.title", "Presets" },
            { "preset.management", "Preset Management" },
            { "preset.refresh", "Refresh" },
            { "preset.empty", "No saved presets." },
            { "preset.save_current", "Save Current Settings" },
            { "preset.new_name", "New Preset {0}" },
            { "preset.apply", "Apply" },
            { "preset.rename", "Rename" },
            { "preset.delete", "Delete" },
            { "preset.export", "Export" },
            { "preset.import", "Import" },
            { "preset.default_name", "Default Settings" },
            { "preset.default_desc", "lilToon default values" },

            // ===== Dialogs =====
            { "dialog.result", "Result" },
            { "dialog.complete", "Complete" },
            { "dialog.warning", "Warning" },
            { "dialog.error", "Error" },
            { "dialog.confirm", "Confirm" },
            { "dialog.apply", "Apply" },
            { "dialog.cancel", "Cancel" },
            { "dialog.close", "Close" },
            { "dialog.select", "Select" },
            { "dialog.save", "Save" },
            { "dialog.change", "Change" },
            { "dialog.overwrite", "Overwrite" },
            { "dialog.duplicate", "Duplicate" },

            // ===== Dialog Messages =====
            { "dialog.all_materials_match", "All material values are identical." },
            { "dialog.different_materials", "Materials with different values: {0}" },
            { "dialog.unify_title", "Unify Materials" },
            { "dialog.unify_message", "Apply selected property values to all lilToon materials.\nThis action cannot be undone." },
            { "dialog.unify_complete", "Material properties have been unified." },
            { "dialog.duplicate_title", "Duplicate Avatar" },
            { "dialog.duplicate_message", "Duplicates avatar and all lilToon materials.\n\n• Creates duplicated avatar\n• Saves materials to new folder\n• Moves duplicate X-0.5" },
            { "dialog.duplicate_unify_title", "Duplicate Avatar and Unify Properties" },
            { "dialog.duplicate_unify_message", "Duplicates avatar and materials, then applies properties.\n\n• Creates duplicated avatar\n• Unifies material properties\n• Moves duplicate X-0.5" },
            { "dialog.duplicate_unify_button", "Duplicate & Unify" },
            { "dialog.delete", "Delete" },
            { "dialog.delete_all_title", "Delete Duplicated Avatar & Materials" },
            { "dialog.delete_all_message", "Deletes the currently selected duplicated avatar and material folder.\n\nThis action cannot be undone." },
            { "dialog.delete_all_complete", "Deletion complete!\n\nAvatar: {0}\nMaterial folder: {1}" },
            { "dialog.delete_avatar_complete", "Avatar '{0}' has been deleted." },
            { "dialog.not_duplicated_avatar", "Only duplicated avatars (_Copy) can be deleted." },
            { "dialog.no_liltoon", "No objects using lilToon shader found." },
            { "dialog.apply_complete", "Brightness control has been applied!\n\nApplied features (number of objects):\n{0}" },

            // ===== Preset Dialogs =====
            { "preset.save_title", "Save New Preset" },
            { "preset.name_label", "Name" },
            { "preset.desc_label", "Description" },
            { "preset.name_required", "Please enter a preset name." },
            { "preset.rename_title", "Rename Preset" },
            { "preset.new_name_label", "New Name" },
            { "preset.overwrite_title", "Overwrite Preset" },
            { "preset.overwrite_message", "Preset '{0}' already exists.\nDo you want to overwrite?" },
            { "preset.save_complete_title", "Save Complete" },
            { "preset.save_complete_message", "Preset '{0}' has been saved." },
            { "preset.applied_log", "[Preset] '{0}' preset applied (Individual materials: {1})" },
            { "preset.delete_title", "Delete Preset" },
            { "preset.delete_message", "Delete preset '{0}'?\nThis action cannot be undone." },
            { "preset.export_select", "Please select a preset to export first." },
            { "preset.export_title", "Export Preset" },
            { "preset.export_complete_title", "Export Complete" },
            { "preset.export_complete_message", "Preset has been saved:\n{0}" },
            { "preset.import_title", "Import Preset" },
            { "preset.import_complete_title", "Import Complete" },
            { "preset.import_complete_message", "Preset '{0}' has been imported." },

            // ===== Buttons =====
            { "button.apply", "Apply" },
            { "button.select_avatar", "Please select an avatar" },
            { "button.select_feature", "Please select at least one feature" },

            // ===== Cloth Editor =====
            { "cloth.title", "Toggle Groups" },
            { "cloth.selected_count", "Items: {0}" },
            { "cloth.no_items", "Add a group to start" },
            { "cloth.settings", "Settings" },
            { "cloth.saved_state", "Save State" },
            { "cloth.apply", "Create Toggles" },
            { "cloth.select_items", "Please add items" },
            { "cloth.apply_complete", "{0} cloth toggles have been created!" },
            { "cloth.drop_here", "Drop items here" },
            { "cloth.default_index", "Default" },
            { "cloth.target_menu", "Target Menu" },
            { "cloth.add_all_off", "Add All OFF" },
            { "cloth.toggle_name", "Parameter" },
            { "cloth.save_folder", "Save Folder" },
            { "cloth.default_folder", "(Default location)" },
            { "cloth.select_folder", "Select Save Folder" },
            { "cloth.folder_error", "Only folders inside Assets can be selected." },
            { "cloth.new_menu", "Create New Menu" },

            // ===== Update =====
            { "update.available", "→ v{0} update available (Check VCC)" },

            // ===== Language =====
            { "language.title", "Language" },
        };
    }
}
