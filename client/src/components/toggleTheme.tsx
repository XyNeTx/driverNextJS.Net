// 'use client';
// import { useEffect,useState } from "react";


// export default function toggleTheme() : string {
//     const [theme,setTheme] = useState('light');

//     useEffect(() => {
//         const savedTheme = localStorage.getItem('theme') || 'dark';
//         setTheme(savedTheme);
//         document.documentElement.setAttribute('data-theme', savedTheme);
//     }, []);

//     const toggle = () => {
//         const newTheme = theme === 'dark' ? 'light' : 'dark';
//         localStorage.setItem('theme', newTheme);
//         setTheme(newTheme);
//         document.documentElement.setAttribute('data-theme', newTheme);
//     };

//     return theme;
// }

