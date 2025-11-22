import type { NextConfig } from "next";

const isProd = process.env.NODE_ENV === 'production';

const nextConfig: NextConfig = {
  /* config options here */
  output: 'export',
  trailingSlash: false,
  basePath: isProd ? '/NewDriver' : '', // ✅ Matches your IIS virtual path
  assetPrefix: isProd ? '/NewDriver' : '', // ✅ Ensures assets use the virtual path
  //basePath: '/NewDriver',
  //assetPrefix: '/NewDriver'
};

export default nextConfig;
