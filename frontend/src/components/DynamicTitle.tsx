"use client";
import { pageTitle } from "@/constants/pageTitle";
import { usePathname } from "next/navigation";
import { useEffect, useMemo } from "react";

export default function DynamicTitle() {
    const isProd = process.env.NODE_ENV === 'production';
    const pathname = isProd ? usePathname().slice(0,usePathname.length - 1) : usePathname();
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