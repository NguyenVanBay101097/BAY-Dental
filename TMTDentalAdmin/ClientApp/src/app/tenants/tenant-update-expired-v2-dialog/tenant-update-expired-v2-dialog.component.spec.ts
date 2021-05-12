import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantUpdateExpiredV2DialogComponent } from './tenant-update-expired-v2-dialog.component';

describe('TenantUpdateExpiredV2DialogComponent', () => {
  let component: TenantUpdateExpiredV2DialogComponent;
  let fixture: ComponentFixture<TenantUpdateExpiredV2DialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantUpdateExpiredV2DialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantUpdateExpiredV2DialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
