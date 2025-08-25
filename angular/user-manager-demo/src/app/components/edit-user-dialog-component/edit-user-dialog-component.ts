import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-edit-user-dialog-component',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule],
  templateUrl: './edit-user-dialog-component.html',
  styleUrl: './edit-user-dialog-component.css'
})
export class EditUserDialogComponent {
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EditUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { user: any }
  ) {
    this.form = this.fb.group({
      firstName: [data.user.firstName, Validators.required],
      lastName: [data.user.lastName, Validators.required],
      phoneNumber: [data.user.phoneNumber],
      zipCode: [data.user.zipCode]
    });
  }

  save() {
    if (this.form.valid) {
      this.dialogRef.close({ ...this.data.user, ...this.form.value });
    }
  }

  close() {
    this.dialogRef.close();
  }
}
