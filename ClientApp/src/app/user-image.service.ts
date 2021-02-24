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
  userImages: any;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string)
  {
    this.baseUrl = baseUrl;
  }


  // Get the User Images
  getUserImages() {

    // Use the API to get the user images
    return this.http.get(this.baseUrl + 'api/UserImages');
      //.subscribe(result => {
      //  console.log(result);
      //  this.userImages = result;
      //});

    //return this.userImages;
  } 
}
