using System.Collections.Generic;

namespace Brightness.Localization
{
    public static class Korean
    {
        public static readonly Dictionary<string, string> Strings = new()
        {
            // ===== Menu & Window =====
            { "menu.title", "밝기 조절 에디터" },
            { "window.title", "Sodanen 에디터" },

            // ===== Avatar Section =====
            { "avatar.title", "아바타" },
            { "avatar.target", "대상" },
            { "avatar.select", "아바타를 선택하세요" },
            { "avatar.select_dropdown", "-- 아바타 선택 --" },
            { "avatar.material_count", "lilToon 마테리얼: {0}개" },
            { "avatar.no_descriptor", "VRCAvatarDescriptor 없음" },
            { "avatar.menu_location", "메뉴" },
            { "avatar.menu_root", "루트 메뉴" },

            // ===== Feature Section =====
            { "feature.title", "기능 선택" },
            { "feature.light", "Light" },
            { "feature.shadow", "Shadow" },
            { "feature.minlight", "Min Light" },
            { "feature.maxlight", "Max Light" },
            { "feature.backlight", "Back Light" },
            { "feature.shadow_main", "Shadow" },
            { "feature.shadow_x", "Shadow X Angle" },
            { "feature.shadow_y", "Shadow Y Angle" },
            { "feature.minlight.tooltip", "어두운 환경에서의 최소 밝기 제한" },
            { "feature.maxlight.tooltip", "밝은 환경에서의 최대 밝기 제한" },
            { "feature.backlight.tooltip", "뒤에서 오는 빛의 강도" },
            { "feature.shadow.tooltip", "그림자 방향 조절" },
            { "feature.shadow_x.tooltip", "X축 그림자 방향" },
            { "feature.shadow_y.tooltip", "Y축 그림자 방향" },

            // ===== Material Settings =====
            { "material.title", "마테리얼 설정" },
            { "material.window_title", "Sodanen Material Editor" },
            { "material.unify_toggle", "속성 통일 / 개별 조절" },
            { "material.select_all", "전체" },
            { "material.deselect_all", "해제" },

            // ===== Unify Section =====
            { "unify.title", "마테리얼 속성 통일" },
            { "unify.check_before", "적용 전에 마테리얼 초기값이 동일한지 확인" },
            { "unify.source_material", "마테리얼에서 가져오기" },
            { "unify.shadow", "그림자" },
            { "unify.mask_strength", "마스크와 강도" },
            { "unify.strength", "강도" },
            { "unify.shadow_1", "그림자 색 1" },
            { "unify.shadow_2", "그림자 색 2" },
            { "unify.shadow_3", "그림자 색 3" },
            { "unify.color", "색상" },
            { "unify.range", "범위" },
            { "unify.blur", "흐리게" },
            { "unify.normal_strength", "노멀 맵 강도" },
            { "unify.receive_shadow", "그림자를받는" },
            { "unify.alpha", "투명도" },
            { "unify.border_color", "경계의 색" },
            { "unify.border_range", "경계의 폭" },
            { "unify.contrast", "콘트라스트" },
            { "unify.env_strength", "그림자 색에 환경 광의 영향" },
            { "unify.blur_mask", "흐림 효과 마스크" },
            { "unify.compare", "수정 전 수치 비교" },
            { "unify.apply_material", "마테리얼 적용" },

            // ===== Non-Destructive Mode =====
            { "nondestructive.title", "비파괴 모드" },
            { "nondestructive.description", "아바타와 마테리얼을 복제합니다. 원본은 변경되지 않습니다." },
            { "nondestructive.duplicate_only", "복제만" },
            { "nondestructive.duplicate_unify", "복제 + 통일" },
            { "nondestructive.delete_all", "복제 삭제" },

            // ===== Custom Material =====
            { "custom.title", "개별 마테리얼 그림자 조절 ({0}개)" },
            { "custom.add_help", "+ 버튼으로 마테리얼을 추가하세요" },
            { "custom.exclude_notice", "이 마테리얼들은 '마테리얼 속성 통일'에서 제외됩니다" },
            { "custom.select_material", "(마테리얼 선택)" },
            { "custom.material", "마테리얼" },
            { "custom.current_value", "현재값" },
            { "custom.shadow_1", "그림자 1" },
            { "custom.shadow_2", "그림자 2" },
            { "custom.shadow_3", "그림자 3" },

            // ===== Preset Section =====
            { "preset.title", "프리셋" },
            { "preset.management", "프리셋 관리" },
            { "preset.refresh", "새로고침" },
            { "preset.empty", "저장된 프리셋이 없습니다." },
            { "preset.save_current", "현재 설정 저장" },
            { "preset.new_name", "새 프리셋 {0}" },
            { "preset.apply", "적용" },
            { "preset.rename", "이름 변경" },
            { "preset.delete", "삭제" },
            { "preset.export", "내보내기" },
            { "preset.import", "가져오기" },
            { "preset.default_name", "기본 설정" },
            { "preset.default_desc", "lilToon 기본 설정값" },

            // ===== Dialogs =====
            { "dialog.result", "결과" },
            { "dialog.complete", "완료" },
            { "dialog.warning", "경고" },
            { "dialog.error", "오류" },
            { "dialog.confirm", "확인" },
            { "dialog.apply", "적용" },
            { "dialog.cancel", "취소" },
            { "dialog.close", "닫기" },
            { "dialog.select", "선택" },
            { "dialog.save", "저장" },
            { "dialog.change", "변경" },
            { "dialog.overwrite", "덮어쓰기" },
            { "dialog.duplicate", "복제" },

            // ===== Dialog Messages =====
            { "dialog.all_materials_match", "모든 마테리얼의 수치가 동일합니다." },
            { "dialog.different_materials", "수치가 다른 마테리얼: {0}개" },
            { "dialog.unify_title", "마테리얼 통일" },
            { "dialog.unify_message", "선택된 속성값을 모든 lilToon 마테리얼에 적용합니다.\n이 작업은 되돌릴 수 없습니다." },
            { "dialog.unify_complete", "마테리얼 속성이 통일되었습니다." },
            { "dialog.duplicate_title", "아바타 복제" },
            { "dialog.duplicate_message", "아바타와 모든 lilToon 마테리얼을 복제합니다.\n\n• 복제된 아바타 생성\n• 마테리얼 새 폴더에 저장\n• 복제 아바타 X축 -0.5 이동" },
            { "dialog.duplicate_unify_title", "아바타 복제 및 속성 통일" },
            { "dialog.duplicate_unify_message", "아바타와 마테리얼을 복제하고 속성을 적용합니다.\n\n• 복제된 아바타 생성\n• 마테리얼에 속성 통일\n• 복제 아바타 X축 -0.5 이동" },
            { "dialog.duplicate_unify_button", "복제 및 통일" },
            { "dialog.delete", "삭제" },
            { "dialog.delete_all_title", "복제 아바타 및 마테리얼 삭제" },
            { "dialog.delete_all_message", "현재 선택된 복제 아바타와 마테리얼 폴더를 삭제합니다.\n\n이 작업은 되돌릴 수 없습니다." },
            { "dialog.delete_all_complete", "삭제 완료!\n\n아바타: {0}\n마테리얼 폴더: {1}" },
            { "dialog.delete_avatar_complete", "아바타 '{0}'이(가) 삭제되었습니다." },
            { "dialog.not_duplicated_avatar", "복제된 아바타(_Copy)만 삭제할 수 있습니다." },
            { "dialog.no_liltoon", "lilToon 쉐이더를 사용하는 오브젝트를 찾을 수 없습니다." },
            { "dialog.apply_complete", "밝기 조절 기능이 적용되었습니다!\n\n적용된 기능 (적용된 오브젝트 수):\n{0}" },

            // ===== Preset Dialogs =====
            { "preset.save_title", "새 프리셋 저장" },
            { "preset.name_label", "이름" },
            { "preset.desc_label", "설명" },
            { "preset.name_required", "프리셋 이름을 입력해주세요." },
            { "preset.rename_title", "프리셋 이름 변경" },
            { "preset.new_name_label", "새 이름" },
            { "preset.overwrite_title", "프리셋 덮어쓰기" },
            { "preset.overwrite_message", "'{0}' 프리셋이 이미 존재합니다.\n덮어쓰시겠습니까?" },
            { "preset.save_complete_title", "저장 완료" },
            { "preset.save_complete_message", "프리셋 '{0}'이(가) 저장되었습니다." },
            { "preset.applied_log", "[Preset] '{0}' 프리셋 적용됨 (개별 마테리얼: {1}개)" },
            { "preset.delete_title", "프리셋 삭제" },
            { "preset.delete_message", "'{0}' 프리셋을 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다." },
            { "preset.export_select", "먼저 내보낼 프리셋을 선택해주세요." },
            { "preset.export_title", "프리셋 내보내기" },
            { "preset.export_complete_title", "내보내기 완료" },
            { "preset.export_complete_message", "프리셋이 저장되었습니다:\n{0}" },
            { "preset.import_title", "프리셋 가져오기" },
            { "preset.import_complete_title", "가져오기 완료" },
            { "preset.import_complete_message", "프리셋 '{0}'이(가) 가져왔습니다." },

            // ===== Buttons =====
            { "button.apply", "적용하기" },
            { "button.select_avatar", "아바타를 선택해주세요" },
            { "button.select_feature", "최소 하나의 기능을 선택해주세요" },

            // ===== Cloth Editor =====
            { "cloth.title", "토글 그룹" },
            { "cloth.selected_count", "등록된 아이템: {0}개" },
            { "cloth.no_items", "그룹을 추가해주세요" },
            { "cloth.settings", "설정" },
            { "cloth.saved_state", "상태 저장" },
            { "cloth.apply", "Toggle 생성" },
            { "cloth.select_items", "아이템을 추가해주세요" },
            { "cloth.apply_complete", "{0}개의 의상 토글이 생성되었습니다!" },
            { "cloth.drop_here", "여기에 드래그 앤 드롭" },
            { "cloth.default_index", "기본값" },
            { "cloth.target_menu", "대상 메뉴" },
            { "cloth.add_all_off", "All OFF 추가" },
            { "cloth.toggle_name", "파라미터" },
            { "cloth.save_folder", "저장 폴더" },
            { "cloth.default_folder", "(기본 위치)" },
            { "cloth.select_folder", "저장 폴더 선택" },
            { "cloth.folder_error", "Assets 폴더 내부만 선택할 수 있습니다." },
            { "cloth.new_menu", "새 메뉴 생성" },

            // ===== Update =====
            { "update.available", "→ v{0} 업데이트 가능 (VCC 확인)" },

            // ===== Language =====
            { "language.title", "언어" },
        };
    }
}
