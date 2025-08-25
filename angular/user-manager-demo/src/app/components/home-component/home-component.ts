import { CommonModule, AsyncPipe, NgForOf } from '@angular/common';
import { Component, inject, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../../services/auth.service';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from "@angular/material/input";
import { RouterModule } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { UserService } from '../../services/user.service';
import { PagedResult } from '../../models/paged-result.model';
import { User } from '../../models/user.model';
import { ReadUser } from '../../models/read-user.model';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { EditUserDialogComponent } from '../edit-user-dialog-component/edit-user-dialog-component';
import { MatDialog } from '@angular/material/dialog';
import { DeleteUserDialogComponent } from '../delete-user-dialog-component/delete-user-dialog-component';

@Component({
  selector: 'app-home-component',
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatListModule,
    MatInputModule,
    RouterModule,
    MatMenuModule,
    MatPaginatorModule
  ],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css'
})
export class HomeComponent {


  opened = true;
  auth = inject(AuthService);
  userService = inject(UserService);
  dialog = inject(MatDialog);
  displayedColumns: string[] = ['email', 'firstName', 'lastName', 'phoneNumber', 'zipCode', 'actions'];
  totalCount = 0;
  pageIndex = 1;
  pageSize = 10;
  filterValue = ''
  users: ReadUser[] = [];
  filteredUsers: ReadUser[] = [];
  dataSource = new MatTableDataSource<ReadUser>();
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;


  ngOnInit() {
    this.loadUsers();
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  logout() {
    this.auth.logout().subscribe();
  }

  loadUsers(): void {
    this.userService.getPaged(this.pageIndex, this.pageSize)
      .subscribe(result => {
        if (result.isSuccess) {
          const paged: PagedResult<ReadUser> | undefined = result.value;
          if (paged) {
            this.dataSource.data = paged.items;
            this.totalCount = paged.totalCount;
            this.pageIndex = paged.pageIndex;
            this.pageSize = paged.pageSize;

            this.dataSource.filterPredicate = (data: { firstName: string; lastName: string; email: string; }, filter: string) => {
              const term = filter.trim().toLowerCase();
              return data.firstName.toLowerCase().includes(term)
                || data.lastName.toLowerCase().includes(term)
                || data.email.toLowerCase().includes(term);
            };
          }
        }
      });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadUsers();
  }
  editUser(user: ReadUser) {
    this.dialog.open(EditUserDialogComponent, {
      data: { user }
    }).afterClosed().subscribe(result => {
      if (result) {
        this.userService.updateUser(user.id.toString(), result)
        .subscribe({
          next: () => {
            this.loadUsers(); // chắc chắn gọi sau khi update xong
          },
          error: err => {
            console.error('Update failed', err);
          }
        })
      }
    });
  }

  deleteUser(user: ReadUser) {
    this.dialog.open(DeleteUserDialogComponent, {
      data: {
        title: 'Confirm Delete',
        message: 'Are you sure you want to delete this user?'
      }
    }).afterClosed().subscribe(result => {
      if (result) {
        this.userService.deleteUser(user.id.toString())
          .subscribe({
          next: () => {
            this.loadUsers();
          },
          error: err => {
            console.error('Delete failed', err);
          }
        })
      }
    });
  }
}

