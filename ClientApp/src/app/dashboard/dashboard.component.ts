import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { Profile, UserManager } from 'oidc-client';
import { from, Observable } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
import { ApplicationName, ApplicationPaths } from '../../api-authorization/api-authorization.constants';
import { AuthorizeService, IUser } from '../../api-authorization/authorize.service';
import { UserImageService } from '../user-image.service';
import { UserImage } from '../user-image';
import { Router } from '@angular/router';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  private userManager: UserManager;
  public isAuthenticated: Observable<boolean>;
  public userName: Observable<string>;
  public accessToken: Observable<string>;
  public userId: string;
  public user: Observable<IUser>;
  public userProfile: Observable<string>;
  private baseUrl: string;

  public progress: number = 0;

  userImages: any = [];

  constructor(private authorizeService: AuthorizeService,
    private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    private router: Router,
    private userImageService: UserImageService) {
    this.baseUrl = baseUrl;
  }

  // Hold all the selected files
  selectedFiles: File[] = null;

   async ngOnInit() {
    //this.isAuthenticated = this.authorizeService.isAuthenticated();
    this.userName = this.authorizeService.getUser().pipe(map(u => u && u.name));
    this.user = this.authorizeService.getUser();
    //this.accessToken = this.authorizeService.getAccessToken();
    this.authorizeService.getUserId().subscribe(val => this.userId = val);
    //this.userProfile = this.authorizeService.getUserProfile();

     //this.http.get(this.baseUrl + 'api/UserImages').subscribe(result => { console.log(result); this.userImages = result; });
     this.getUserImages();
    //this.userImages = await this.userImageService.getUserImages();
    this.progress = 0;
  }


  onFileSelected(event) {
    console.log(event);
    this.selectedFiles = <File[]>event.target.files;
  }

  async onUpload() {
    const fd = new FormData();

    // Images loaded into the form data
    for (let file of this.selectedFiles) {
      fd.append(file.name, file);
    }

    // User ID to associate with the image
    fd.append('userId', this.userId);

    // POST the data to the server
    // Monitor the progress with the upload
    this.http.post(this.baseUrl + 'api/UserImages', fd, {
      reportProgress: true,
      observe: 'events'
    }).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round(100 * event.loaded / event.total);
      }

      // Wait for the response
      if (event.type == HttpEventType.Response) {
        // Get the latest images uploaded
        this.getUserImages();

        console.log(event);

        // Clear the entry
        this.selectedFiles = null;
      }

    }, error => console.error(error));
  }

  gotoDetails(img): void {
    this.router.navigate(['/image', img.id]);
  }

  getUserImages() {
    this.userImageService.getUserImages().subscribe(result => {
      this.userImages = result

      // Convert the JSON strings to JSON
      //for (var x = 0; x < this.userImages.length; x++) {
      //  this.userImages[x].metaData = JSON.parse(this.userImages[x].metaData);
      //}
    });
  }

  isPortrait(img: UserImage): boolean {
    if (img.orientation == 1) {
      return true;
    }

    return false;
  }

}
