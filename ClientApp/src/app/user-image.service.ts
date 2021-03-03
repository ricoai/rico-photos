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


  // Get the User Images
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

  getUserImage(id: string) {
    return this.http.get(this.baseUrl + 'api/UserImages/' + id)
  }
}
