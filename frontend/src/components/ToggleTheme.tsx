'use client'
import { MoonHalfRight5Bulk, Sun1Bulk } from "@lineiconshq/free-icons";
import { Lineicons } from "@lineiconshq/react-lineicons";
import { useEffect, useState } from "react";

export default function ThemeToggle() {
    const [theme, setTheme] = useState<string>("");

    useEffect(() => {
        const savedTheme = localStorage.getItem("theme");
        const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
        const initialTheme = savedTheme || (prefersDark ? "dark" : "light");
        setTheme(initialTheme);
    }, []);

    useEffect(() => {
        document.documentElement.dataset.theme = theme;
        document.documentElement.style.colorScheme = theme;
        localStorage.setItem("theme", theme);
    }, [theme]);

    return (
        <div className="lg:flex-1 lg:ps-4 justify-content-center">
            <Lineicons className="text-center" icon={theme === "dark" ? Sun1Bulk : MoonHalfRight5Bulk} size={20} onClick={() => setTheme((t) => (t === "light" ? "dark" : "light"))} />
        </div>
  );
}
