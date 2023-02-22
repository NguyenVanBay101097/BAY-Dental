import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantUpdateExpiredDialogComponent } from './tenant-update-expired-dialog.component';

describe('TenantUpdateExpiredDialogComponent', () => {
  let component: TenantUpdateExpiredDialogComponent;
  let fixture: ComponentFixture<TenantUpdateExpiredDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantUpdateExpiredDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantUpdateExpiredDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
