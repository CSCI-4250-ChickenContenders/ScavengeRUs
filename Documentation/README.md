# Repository Branch guide
This is documentation explaining the branches inside this repository for team **Chicken Contenders**
<br>
* **Master**- This is the original code we forked from at the start of our project.
* **Production**- This is a branch that contains each release (*Release-1.1.0* & *Release-1.2.0*). These release branches are tasks that were approved to be release from the PO.
	* **Release-1.X.X**- This is a release branch used to identify the version that has been published to the client or shareholder. Additionally these are the updated versions of the code release to the client at the end of a successful sprint.
*  **Development**-This is a staging branch to use for any releases from the Testing branch. The results of this branch are then pushed into a release branch from Production.
* **Testing**- This branch is used as a testing environment to ensure all the Developers code can merge before the sprint review.
	* **SCRUM1-XX**- This is a task branch that correlates to a task documented in Jira. Each task branch, upon completion, is pushed and merged to "Testing" to be demonstrated for the PO.
