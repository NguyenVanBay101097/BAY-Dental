import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantUpdateInfoDialogComponent } from './tenant-update-info-dialog.component';

describe('TenantUpdateInfoDialogComponent', () => {
  let component: TenantUpdateInfoDialogComponent;
  let fixture: ComponentFixture<TenantUpdateInfoDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantUpdateInfoDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantUpdateInfoDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
