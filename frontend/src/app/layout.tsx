import { Geist, Geist_Mono } from "next/font/google";
import Sidebar from "./sidebar";
import "./globals.css";
import DynamicTitle from "@/components/DynamicTitle";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
        >
        <Sidebar />
        <main className="pl-52 pt-24 p-4 lg:pl-4" >{children}</main>
        <DynamicTitle/>
      </body>
    </html>
  );
}
