import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  postData: any;
  fName: string = '';
  lName: string = '';
  id: number = 0;

  constructor(private http: HttpClient) {}

  getData() {
    this.http.get('https://localhost:7199/Crud').subscribe((data) => {
      this.postData = data;
      console.warn(data);
    });
  }

  onInsert() {
    var data = [{ FirstName: this.fName, LastName: this.lName }];
    this.http.post('https://localhost:7199/Crud', data).subscribe(
      (result) => {
        if (result) {
          console.log('Insert - Success');
        } else {
          console.log('Insert - Failure. May be duplicate.');
        }
      },
      (error) => {
        console.error('error', error);
      }
    );
  }

  onEdit() {
    var data = [{ Id: this.id, FirstName: this.fName, LastName: this.lName }];
    this.http.put('https://localhost:7199/Crud', data).subscribe(
      (result) => {
        if (result) {
          console.log('Edit - Success');
        } else {
          console.log('Edit - Failure. Invalid record');
        }
      },
      (error) => {
        console.error('error', error);
      }
    );
  }

  onDelete() {
    this.http.delete(`https://localhost:7199/Crud/?id=${this.id}`).subscribe(
      (result) => {
        if (result) {
          console.log('Delete - Success');
        } else {
          console.log('Delete - Failure. Invalid record');
        }
      },
      (error) => {
        console.error('error', error);
      }
    );
  }
}
