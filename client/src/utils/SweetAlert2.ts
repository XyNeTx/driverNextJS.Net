import swal from 'sweetalert2';

class option {
    title?: string;
    text?: string;
    icon?: "success" | "error" | "warning" | "info" | "question";
    showCancelButton?: boolean;
    confirmButtonText?: string;
    cancelButtonText?: string;
}

export default async function showSwal(option: option) : Promise<boolean> {
    return await swal.fire({
        title: option.title,
        text: option.text,
        icon: option.icon ?? "info",
        showCancelButton: option.showCancelButton ?? false,
        confirmButtonText: option.confirmButtonText ?? "ตกลง",
        cancelButtonText: option.cancelButtonText ?? "ยกเลิก",
    }).then((result) : boolean => {
        return result.isConfirmed;
    });
}