"use client";
import { useEffect, useState } from "react";

export default function ThemeToggle() {
    const [theme, setTheme] = useState("light");

    useEffect(() => {
        // Runs only on client
        const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
        const savedTheme = localStorage.getItem("theme");

        // Load from localStorage or system
        if (savedTheme) {
            setTheme(savedTheme);
        } else {
            setTheme(prefersDark ? "dark" : "light");
        }
    }, []);

    useEffect(() => {
        // Apply theme to document when state changes
        document.documentElement.dataset.theme = theme;
        document.documentElement.style.colorScheme = theme;
        localStorage.setItem("theme", theme);
    }, [theme]);

    function ThemeSetFunc(){
        setTheme((prev) => (prev === "light" ? "dark" : "light"));
    }

    return (
        <button
            onClick={ThemeSetFunc}
            className="p-2 rounded border"
            >
            Toggle {theme === "light" ? "Dark" : "Light"} Mode
        </button>
    );
};
