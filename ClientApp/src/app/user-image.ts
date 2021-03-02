
export interface UserImage {
  id: number;
  userId: string;
  isPublic: boolean;
  create: Date;
  modified: Date;
  origImageName: string;
  imageName: string;
  isVertical: boolean;
  s3Path: string;
  s3ThumbPath: string;
  fileType: string;
  fileSizeBytes: number;
  fileSizeStr: string;
  metaData: any;
  width: number;
  height: number;
  orientation: number;
  tags: string;
  //albums: number[];
  aiObjectsTags: string;
  aiFacialTags: string;
  aiModerationTags: string;
  aiTextInImageTags: string;
}
