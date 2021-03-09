import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { UserImage } from './user-image';

@Injectable({
  providedIn: 'root'
})
export class UserImageService {

  // Properties
  // Base URL to use API
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string)
  {
    this.baseUrl = baseUrl;
  }

  // *****************************************
  // Test if we can get any data from the database.
  // This will retun the last 10 images if successful.
  // :return: Last 10 images uploaded.
  // *****************************************
  testDbConnection() {
    return this.http.get(this.baseUrl + 'api/UserImages/');
  }

  // *****************************************
  // Get the User Images based on the UserId.
  // This is all the images uploaded from a specific user.
  // :param userId: User ID.
  // :return: All the images for the specific User.
  // *****************************************
  getUserImages(userId: string) {

    //var userImages: any = [];

    // Use the API to get the user images
    /**
    await this.http.get(this.baseUrl + 'api/UserImages')
      .subscribe(result => {
        console.log(result);

        userImages = result;

        // Convert the JSON strings to JSON
        for (var x = 0; x < userImages.length; x++) {
          userImages[x].metaData = JSON.parse(userImages[x].metaData);
        }
        return userImages;
      });
      */

    // Use the API to get the user images
    return this.http.get(this.baseUrl + 'api/UserImages/user/' + userId);

    //return userImages;
  }

  // *****************************************
  // Return a specific image.  This will return the image for the given id.
  // :param id: ID for the image.
  // :return: The image for the given id.
  // *****************************************
  getUserImage(id: string) {
    return this.http.get(this.baseUrl + 'api/UserImages/' + id)
  }
}
