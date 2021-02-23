import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { Profile, UserManager } from 'oidc-client';
import { from, Observable } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
import { ApplicationName, ApplicationPaths } from '../../api-authorization/api-authorization.constants';
import { AuthorizeService, IUser } from '../../api-authorization/authorize.service';

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

  constructor(private authorizeService: AuthorizeService, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  userImages: Object = null;
  selectedFiles: File[] = null;

  ngOnInit() {
    this.isAuthenticated = this.authorizeService.isAuthenticated();
    this.userName = this.authorizeService.getUser().pipe(map(u => u && u.name));
    this.user = this.authorizeService.getUser();
    this.accessToken = this.authorizeService.getAccessToken();
    this.authorizeService.getUserId().subscribe(val => this.userId = val);
    this.userProfile = this.authorizeService.getUserProfile();

    this.http.get(this.baseUrl + 'api/UserImages').subscribe(result => { console.log(result); this.userImages = result; });
    this.progress = 0;
  }


  onFileSelected(event) {
    console.log(event);
    this.selectedFiles = <File[]>event.target.files;
  }

  onUpload() {
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

      console.log(event);

    }, error => console.error(error));
  }

}
