import Image, { ImageProps } from 'next/image';

const isProd = process.env.NODE_ENV === 'production';

interface AppImageProps extends Omit<ImageProps, 'src'> {
  src: string;
  alt: string;
}

export default function AppImage({ src, alt, ...props }: AppImageProps) {
  const basePath = isProd ? '/NewDriver' : '';

  // Add basePath if src starts with /
  const imageSrc = src.startsWith('/') ? `${basePath}${src}` : src;

  return <Image src={imageSrc} alt={alt} {...props} unoptimized />;
}