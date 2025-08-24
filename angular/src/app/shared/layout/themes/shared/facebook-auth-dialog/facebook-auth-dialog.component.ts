import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'facebook-auth-dialog',
  template: `
    <div class="auth-backdrop" (click)="close()"></div>
    <div class="auth-container">
      <iframe
        [src]="authUrl"
        width="100%"
        height="500"
        frameborder="0"
        allow="public_profile"
      ></iframe>
    </div>
  `,
  styleUrls: ['./facebook-auth-dialog.component.css']
})
export class FacebookAuthDialogComponent implements OnInit {
  @Input() authUrl: string;

  constructor() {}

  ngOnInit(): void {}

  close() {
    document.body.classList.remove('show-facebook-dialog');
  }
}
