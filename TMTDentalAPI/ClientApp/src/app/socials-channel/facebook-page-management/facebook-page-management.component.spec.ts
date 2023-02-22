import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageManagementComponent } from './facebook-page-management.component';

describe('FacebookPageManagementComponent', () => {
  let component: FacebookPageManagementComponent;
  let fixture: ComponentFixture<FacebookPageManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
