const isProd = process.env.NODE_ENV === 'production';
const prodLink = isProd ? process.env.NEXT_PUBLIC_PROD_API_LINK : "http://localhost:5272"

export const APIURL = prodLink;