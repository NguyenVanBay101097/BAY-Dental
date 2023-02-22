import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookSelectTagComponent } from './facebook-select-tag.component';

describe('FacebookSelectTagComponent', () => {
  let component: FacebookSelectTagComponent;
  let fixture: ComponentFixture<FacebookSelectTagComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookSelectTagComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookSelectTagComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
