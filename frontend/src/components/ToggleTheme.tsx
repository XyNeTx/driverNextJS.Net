import { Home2Outlined, IconData, MoonHalfRight5Bulk, Sun1Bulk } from "@lineiconshq/free-icons";
import { Lineicons } from "@lineiconshq/react-lineicons";
import { useEffect, useState } from "react";

export default function ThemeToggle() {
    const [theme, setTheme] = useState<string>("");

    useEffect(() => {
        const savedTheme = localStorage.getItem("theme");
        const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
        const initialTheme = savedTheme || (prefersDark ? "dark" : "light");
        setTheme(initialTheme);
        //document.documentElement.dataset.theme = theme;
        //document.documentElement.style.colorScheme = theme;
    }, []);

    useEffect(() => {
        document.documentElement.dataset.theme = theme;
        document.documentElement.style.colorScheme = theme;
        localStorage.setItem("theme", theme);
    }, [theme]);

    return (
        <div className="flex-1 ps-4 ">
            {/* <a href="#"
            className="ms-4 mr-8 h-1"> */}
            {/* Toggle {theme === "light" ? "Dark" : "Light"} Mode */}
            <Lineicons icon={theme === "dark" ? Sun1Bulk : MoonHalfRight5Bulk} size={20} onClick={() => setTheme((t) => (t === "light" ? "dark" : "light"))} />
            {/* </a> */}
        </div>
  );
}
