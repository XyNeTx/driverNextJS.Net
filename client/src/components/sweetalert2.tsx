'use client';
import swal from 'sweetalert2';


class option {
    title?: string;
    text?: string;
    icon?: "success" | "error" | "warning" | "info" | "question";
    showCancelButton?: boolean;
    confirmButtonText?: string;
    cancelButtonText?: string;
}

export default function showSwal(option: option) {
    return swal.fire({
        title: option.title,
        text: option.text,
        icon: option.icon,
        showCancelButton: option.showCancelButton ?? false,
        confirmButtonText: option.confirmButtonText ?? "ตกลง",
        cancelButtonText: option.cancelButtonText ?? "ยกเลิก",
    }).then((result) => {
        return result;
    });
}