import type { NextConfig } from "next";

const isProd = process.env.NODE_ENV === 'production';

const nextConfig: NextConfig = {
  /* config options here */
  output: 'export',
  trailingSlash: false,
  basePath: isProd ? '/DriverApi' : '', // ✅ Matches your IIS virtual path
  assetPrefix: isProd ? '/DriverApi' : '', // ✅ Ensures assets use the virtual path
  //basePath: '/NewDriver',
  //assetPrefix: '/NewDriver'
};

export default nextConfig;
