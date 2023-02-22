import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookConfigEstablishComponent } from './facebook-config-establish.component';

describe('FacebookConfigEstablishComponent', () => {
  let component: FacebookConfigEstablishComponent;
  let fixture: ComponentFixture<FacebookConfigEstablishComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookConfigEstablishComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookConfigEstablishComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
