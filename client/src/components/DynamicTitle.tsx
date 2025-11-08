"use client";
import { pageTitle } from "@/constants/pageTitle";
import { usePathname } from "next/navigation";
import { useEffect, useMemo } from "react";

export default function DynamicTitle() {
    const pathname = usePathname();
    // Find the last segment
    const lastSegment = useMemo(() => pathname.split("/").pop() ?? "", [pathname]);

    // Find the matching title
    const currentPage = useMemo(
        () =>
        pageTitle.find(
            (p) => p.urlName.toLowerCase().includes(lastSegment.toLowerCase())
        ),
        [lastSegment]
    );

    useEffect(() => {
        document.title = currentPage?.title ?? "Driver Tracking";

        // Optional: console.log for debug
        // console.log("Updated title:", document.title);
    }, [currentPage]);

    return null; // No visible UI needed
}