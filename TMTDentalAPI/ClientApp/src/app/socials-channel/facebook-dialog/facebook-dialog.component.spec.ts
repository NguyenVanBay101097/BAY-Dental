import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookDialogComponent } from './facebook-dialog.component';

describe('FacebookDialogComponent', () => {
  let component: FacebookDialogComponent;
  let fixture: ComponentFixture<FacebookDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
