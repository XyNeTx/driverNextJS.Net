import axios, { AxiosRequestConfig, AxiosResponse } from "axios";

const isProd = process.env.NODE_ENV === 'production';
const prodLink = process.env.NEXT_PUBLIC_PROD_API_LINK;


export default async function CallAxios<T>(option : AxiosRequestConfig) : Promise<T> {
    console.log(prodLink);
    option.url = isProd ? prodLink! + option.url : 'http://localhost:5272' + option.url;
    try
    {
        const response : AxiosResponse<T> = await axios(option);
        return response.data;
    }
    catch (error: unknown)
    {
        if (axios.isAxiosError(error)) {
            console.error("Axios Error:", error.response?.data || error.message);
        } else {
            console.error("Unknown Error:", error);
        }
        throw error;
    }
}