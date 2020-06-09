import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageListComponent } from './facebook-page-list.component';

describe('FacebookPageListComponent', () => {
  let component: FacebookPageListComponent;
  let fixture: ComponentFixture<FacebookPageListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
