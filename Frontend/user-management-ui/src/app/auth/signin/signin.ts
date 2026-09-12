import { Component } from '@angular/core';

@Component({
  selector: 'app-signin',
  imports: [],
  templateUrl: './signin.html',
  styleUrl: './signin.css',
})
export class Signin {

    FormSubmit(event: SubmitEvent) {
      event.preventDefault();
      console.log(event);
      console.log("Form is submitted");
    }
}
